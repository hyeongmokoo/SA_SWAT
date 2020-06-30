##Functions
errorband <- function(x, width){ ##Functions for epsilon band
  if(width>0){
    w <- matrix(1,nrow=width, ncol=width) 
    r <- focal(x, w=w, fun=max, na.rm = F)
  }
  else if(width==0){
    r <- x
  }
  else{
    width <- -1*width
    w <- matrix(1,nrow=width, ncol=width) 
    r <- focal(x, w=w, fun=min, na.rm = F)
  }
  return(r)
}


