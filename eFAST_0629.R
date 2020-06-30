rm(list = ls())
##Required libraries
library(raster)
library(sensitivity)

setwd("C:\\SWAT\\ProgTest\\") ##Change this for local setting.
source("Functions.R") ##Required functions.

##Define Swat preparation program ##Developed by OpenGMS team in C#
prefname <- c("ErrorProp/ErrorProp/bin/Debug/ErrorProp.exe")

##SWAT Batch generation, See Joseph and Guillaume (2013) in Environmental Modelling & Software(46).
#folder_name<-c("output")
bfname<-c("xgeswat\\Scenarios\\Default\\runSample.bat")
#  Define a template for the text of each batch file, and write one batch file 
#  for each folder to be created. 
# Creating batch file with swat2012 and copied swat_edit and absolute_values from the example above
batch.text.template<-'@echo off
C: 
cd "C:\\SWAT\\ProgTest\\xgeswat\\Scenarios\\Default\\TxtInOut"
start /w SWAT_Edit.exe
swat2012.exe
if %errorlevel% == 0 exit 0
echo.'

write(batch.text.template, bfname)

##For checking the SWAT results to verify the whole processes
old.file.mtime <- file.mtime("xgeswat\\Scenarios\\Default\\TxtInOut\\file.cio")
output.rch.file<-as.character(c("xgeswat\\Scenarios\\Default\\TxtInOut\\output.rch"))
output.hru.file<-as.character(c("xgeswat\\Scenarios\\Default\\TxtInOut\\output.hru"))

##Input uncertainty setting
sample.dem <- raster('xgeswat\\Source\\original\\dem.tif') ##Original DEM
sample.landuse <- raster('xgeswat\\Source\\original\\landuse.tif') ##Orginal LULC
ori.lu.res <- xres(sample.landuse)
sample.soil <- raster('xgeswat\\Source\\original\\soil.tif') ##Original Soil datasets
ori.so.res <- xres(sample.soil)

##DEM error preparation ##Loading pregenerated uncertianty datasets to reduce processing time
##These reulsts can be obatined from "DEMPropagation0630.R", currently not exist in GitHub
seq.dem1 <- readRDS(file = 'xgeswat\\Source\\original\\seq600.Rds') ##Seperated datasets to reduce computing burden.
seq.dem2 <- readRDS(file = 'xgeswat\\Source\\original\\seq700.Rds')
seq.dem3 <- readRDS(file = 'xgeswat\\Source\\original\\seq6001.Rds')
seq.dem4 <- readRDS(file = 'xgeswat\\Source\\original\\seq7002.Rds')
seq.dem5 <- readRDS(file = 'xgeswat\\Source\\original\\seq6003.Rds')
seq.dem6 <- readRDS(file = 'xgeswat\\Source\\original\\seq7004.Rds')


rnd.seq.dem <- sample(1:1125) #Genrate random number to avoid using same dataset in same order. 1125 is #of simulation, need to be changed.

#Precipitation error preparation
pcp.file <- as.character(c("xgeswat\\Source\\323103ori.pcp")) ##Original precipitation 
pcp.out.file <- as.character(c("xgeswat\\Source\\323103.pcp")) ##Precipitation for checking process

##Get precipitation and change it Vector
pcp.head <- readLines(pcp.file, n=4)
pp<-readLines(pcp.file)
pp.n <- length(pp)-4
pcp.ori <- vector(mode = 'numeric', length = pp.n)
pcp.unc <- vector(mode = 'character', length = pp.n)
year <- 2013

for(k in 1:pp.n){
  pcp.ori[k] <- as.numeric(pp[k+4])
}

###########Define ranges for SA
##Watershed delineation
UDEM <- list(min = 0, max = 16) ##Parameter 1
MinStream <- list(min = 5000, max = 15000) ##Parameter 2

##HRU creation
ULULC <- list(min = -2000, max = 2000) ##Parameter 3
USOIL <- list(min = -2500, max = 2500) ##Parameter 4
MinLU <- list(min = 5, max = 42) ##Parameter 5
MinSoil <- list(min = 5, max = 50)##Parameter 6
MinSlope <- list(min = 5, max = 50)##Parameter 7
IntSlope <- list(min = 5, max = 80) ##Parameter 8 

##SWAT setup and running
#Precipitation
UPREC <- list(min = -15.2, max = 15.2) ##Parameter 9
##New SWAT parameter
ALPHA_BF <- list(min = 0, max = 1) ##P10
GW_DELAY <- list(min = 0, max = 500) ##P11
CN2 <- list(min = -0.15, max = 0.15)  ##P12
ESCO <- list(min = 0, max = 1) ##P13
RCN <- list(min = 0, max = 2.5) ##P14
NPERCO <- list(min = 0.01, max = 1) ##P15


##Running MC Simulation for extended FAST
##This decompose "fast99" function in sensitivity package
##Define parameter spaces
null.function <- function(){mean(1)} ##Original SA function require this. any function is okay in this context 
model <- null.function
M <- 4
n <- 214 #Parameters selection for e-FAST (Saltelli, 1999, Technometrics)

X.labels <- c("UDEM", "MinStream", "ULULC", "USOIL","MinLU","MinSoil","MinSlope", "IntSlope", "UPREC", "ALPHA_BF", "GW_DELAY", "CN2", "ESCO", "RCN", "NPERCO")
q.arg <- list(UDEM, MinStream, ULULC, USOIL, MinLU, MinSoil, MinSlope, IntSlope, UPREC, ALPHA_BF, GW_DELAY, CN2, ESCO, RCN, NPERCO)
p <- 15
q <- rep("qunif", p)

omega <- numeric(p)
omega[1] <- floor((n - 1)/(2 * M))
m <- floor(omega[1]/(2 * M))
if (m >= p - 1) {
  omega[-1] <- floor(seq(from = 1, to = m, length.out = p - 
                           1))
}else {
  omega[-1] <- (0:(p - 2))%%m + 1
}

s <- 2 * pi/n * (0:(n - 1))
X <- as.data.frame(matrix(nrow = n * p, ncol = p))
colnames(X) <- X.labels
omega2 <- numeric(p)
for (i in 1:p) {
  omega2[i] <- omega[1]
  omega2[-i] <- omega[-1]
  l <- seq((i - 1) * n + 1, i * n)
  for (j in 1:p) {
    g <- 0.5 + 1/pi * asin(sin(omega2[j] * s))
    X[l, j] <- do.call(q[j], c(list(p = g), q.arg[[j]]))
  }
}


parameters <- X ##This is the defined parameter spaces
i <- 0
error.cnt <- 0
model.sucess <- T
sim.n <- nrow(parameters)
n.var <- 45 ## Number of SWAT output variables 
##Define vectors to store analysis results
output <- vector('numeric', length = sim.n)
total.output <- matrix(0,  nrow= sim.n, ncol=n.var) ##all results
basin.cnt <- vector('numeric', length = sim.n) ##watershed counts
hru.cnt <- vector('numeric', length = sim.n) ##hru counts

while(model.sucess){
  i <- i+1
  
  #P1
  ##Get propagated dem from the predifined datasets
  if(rnd.seq.dem[i] <= 600){
    temp.dem <- raster(seq.dem1[rnd.seq.dem[i]])
    
  }else if(rnd.seq.dem[i] <= 1300){
    temp.dem <- raster(seq.dem2[rnd.seq.dem[i]-600])
  }else if(rnd.seq.dem[i] <= 1900){
    temp.dem <- raster(seq.dem3[rnd.seq.dem[i]-1300])
  }else if(rnd.seq.dem[i] <= 2600){
    temp.dem <- raster(seq.dem4[rnd.seq.dem[i]-1900])
  }else if(rnd.seq.dem[i] <= 3200){
    temp.dem <- raster(seq.dem5[rnd.seq.dem[i]-2600])
  }else{
    temp.dem <- raster(seq.dem6[rnd.seq.dem[i]-3200])
  }
  
  ##p1 random field
  temp.dem@data@values <-  scale(temp.dem@data@values)*(parameters[i, 1]/1.96)
  new.dem <- sample.dem+temp.dem
  writeRaster(new.dem, 'xgeswat\\Source\\dem.tif', overwrite = T) ##input dem
  
  ##Delineation definition (P2)
  write.table(round(parameters[i, 2]), 'xgeswat\\Source\\threshold.txt', quote=FALSE, row.names = F, col.names = F)

  
  #Landuse Error (P3)
  if(parameters[i,3] > 0){
    val.rb.error.lu <- ceiling(parameters[i,3] /ori.lu.res)*2 + 1
  }else if(parameters[i,3]==0){
    val.rb.error.lu <- 0
  }else{
    val.rb.error.lu <- -1*ceiling((parameters[i,3]*-1) /ori.lu.res)*2-1
  }
  new.lu <- errorband(sample.landuse, val.rb.error.lu)
  writeRaster(new.lu, 'xgeswat\\Source\\landuse.tif', overwrite=TRUE)
  
  #Soil Error (P4)
  if(parameters[i,4] > 0){
    val.rb.error.so <- ceiling(parameters[i,4] /ori.so.res)*2 + 1
  }else if(parameters[i,4]==0){
    val.rb.error.so <- 0
  }else{
    val.rb.error.so <- -1*ceiling((parameters[i,4]*-1) /ori.so.res)*2-1
  }
  new.soil <- errorband(sample.soil, val.rb.error.so)
  writeRaster(new.soil, 'xgeswat\\Source\\soil.tif', overwrite=TRUE)
  
  
  ##Landuse min definition (P5)
  write.table(parameters[i, 5], 'xgeswat\\Source\\lumindef.txt', quote=FALSE, row.names = F, col.names = F)
  
  ##Soil min definition (P6)
  write.table(parameters[i, 6], 'xgeswat\\Source\\somindef.txt', quote=FALSE, row.names = F, col.names = F)
  
  ##Slope min definition (P7)
  write.table(parameters[i, 7], 'xgeswat\\Source\\slmindef.txt', quote=FALSE, row.names = F, col.names = F)
  
  ##Slope defintion (P8)
  write.table(round(parameters[i, 8]), 'xgeswat\\Source\\slopdef.txt', quote=FALSE, row.names = F, col.names = F)
  
  #Precipitation error (P9)
  for(k in 1:pp.n){
    ori.value <- pcp.ori[k] - ((year*1000+k)*1000) 
    pcp.unc[k] <- as.character(format(round(pcp.ori[k] + (ori.value*parameters[i,9]/100),1), nsmall = 1))
  }
  write.table(c(pcp.head, pcp.unc), pcp.out.file, quote=FALSE, row.names=FALSE, col.names=FALSE)
  
  
  #Running SWAT preparation tool
  system(prefname, show.output.on.console = F, invisible = F)
  
  
  ##validating process
  new.file.mtime <- file.mtime("xgeswat\\Scenarios\\Default\\TxtInOut\\file.cio")
  
  if(new.file.mtime == old.file.mtime){ 
    print(paste("Not running swat prep in ", i, " loop"))
    error.cnt <- error.cnt+1
    if(error.cnt > 10){
      model.sucess <- F
      break
    }
  }else{
    new.file.mtime <- old.file.mtime
  }
  
  #Creating SWAT datasets for running SWAT
  file.copy(dir("xgeswat\\Scenarios\\Default\\TxtInOut",full.names=T),"xgeswat\\Scenarios\\Default\\TxtInOut\\Backup",recursive=F, overwrite = T) ##Copy new prepared files into backup folder
  model.in.rows<- c("v__ALPHA_BF.gw", "v__GW_DELAY.gw", "r__CN2.mgt", "v__ESCO.hru", "v__RCN.bsn", "v__NPERCO.bsn")
  
  model.in.file <-as.character(c("xgeswat\\Scenarios\\Default\\TxtInOut\\model.in"))
  
  ##P10-13
  sample.n <- c(t(parameters[i,10:15]))
  write.table(sample.n, model.in.file, quote=FALSE, row.names=model.in.rows, col.names=FALSE)
  
  ##Run SWAT
  system(bfname, show.output.on.console = F)
  
  ##Get result
  Qsim.data<-read.table(output.rch.file, skip=9)
  hru.data<-tryCatch({read.table(output.hru.file, skip=9)}, error = function(cond){return(-1)})
  if(is.data.frame(hru.data)){
    hru.cnt[i] <- (nrow(hru.data))/14 #Number of hru, 14 = # (year+1(year summary))+1(Total)
  }else{
    hru.cnt[i] <- hru.data
  }
   
  basin.cnt[i] <- (nrow(Qsim.data))/14 #Number of basins
  
  res.id <- nrow(Qsim.data)-basin.cnt[i]+1
  output[i] <- t(Qsim.data[res.id, 7])
  total.output[i,] <- do.call(cbind, Qsim.data[res.id, 7:ncol(Qsim.data)])
  print(paste("Completed ", i, " of the total ", sim.n, ": ", Sys.time()))
  
  if(i >= sim.n){
    model.sucess <- F
  }
}

##Simulation result
fast99.result <- list(basin.cnt, hru.cnt, output, total.output)

##After simulation.
## define the result to "fast99" class to use tell and related functions
x <- list(model = model, M = M, s = s, omega = omega, X = X, r =fast99.result, call = match.call())
class(x) <- "fast99"


x$y <- x$r[[4]][,1] ##flow
x$y <- x$r[[4]][,12] ##NO3
x$y <- x$r[[4]][,43] ##TP
##Calculate main and interaction effects
tell(x)

##See results
x
plot(x)

