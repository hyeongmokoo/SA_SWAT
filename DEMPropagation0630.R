##Input uncertainty setting
library(gstat)
library(sp)
library(raster)
setwd("C:\\SWAT\\ProgTest\\xgeswat\\Source\\original")
sample.dem <- raster('dem.tif')
plot(sample.dem)
agg.fact <- 6
agg.dem <- aggregate(sample.dem, fact = agg.fact, fun=mean) ##Reduce calculate cost

sample.pts <- as(agg.dem, 'SpatialPointsDataFrame')
v.sample <- variogram(dem~1, sample.pts)
m.sample <- fit.variogram(v.sample, vgm(200000, "Exp", 200000, 1))
plot(v.sample, model = m.sample)

sample.grid <- as(sample.dem, 'SpatialPixelsDataFrame')
g.dummy <- gstat(formula = dem~1, dummy = TRUE, data = sample.pts, model =m.sample, nmax = 20, beta = 5)

SGS.res <- predict(g.dummy, sample.grid, nsim = 600)
saveRDS(SGS.res, "seq600.Rds") ##save res for reducing calculate time.



