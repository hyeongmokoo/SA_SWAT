using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CreateHRU;

namespace HydroResponseUnitGenerator
{
    public class HRU
    {
        private mwSWATGlobals hruGlobals;
        private HRUs hruData;
        private string prjPath;

        private string basedem;
        private string basin;
        private string dInfSlope;
        private string d8Slope;
        private string d8;


        public HRU(HRUs data, mwSWATGlobals globals, string path)
        {
            hruGlobals = globals;
            hruData = data;
            prjPath = path;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init(string elevationPath, string basinPath, string dInfSlopePath, string d8SlopePath, string d8Path)
        {
            string sourcePath = prjPath + "\\Source\\" + System.IO.Path.GetFileNameWithoutExtension(basedem);
            #region
            /*
             * MapWindow的版本
             * YSS的版本是直接传路径进来，解除通过字符串来寻找数据的耦合度
             * 
            hruGlobals.elevationPath = sourcePath + "fel.tif";
            hruGlobals.basinsPath = sourcePath + "w.tif";
            hruGlobals.slopePath = sourcePath + "slp.tif";
            //Accept D8 slope if Dinf slope not available
            if (System.IO.File.Exists(hruGlobals.slopePath) == false)
            {
                hruGlobals.slopePath = sourcePath + "sd8.tif";
            }
            else
            {
                hruGlobals.d8Path = sourcePath + "p.tif";
            }
            */
            #endregion
            basedem = elevationPath;
            basin = basinPath;
            dInfSlope = dInfSlopePath;

            hruGlobals.elevationPath = elevationPath;
            hruGlobals.basinsPath = basinPath;
            hruGlobals.slopePath = dInfSlopePath;
            //Accept D8 slope if Dinf slope not available
            if (System.IO.File.Exists(hruGlobals.slopePath) == false)
            {
                d8Slope = d8SlopePath;
                hruGlobals.slopePath = d8SlopePath;
            }
            else
            {
                d8 = d8Path;
                hruGlobals.d8Path = d8Path;
            }


            hruGlobals.fullHRUsPath = prjPath + "\\Source\\FullHRUs.shp";
            mwSWATDbUtils.GetDbTables(hruGlobals);

            hruGlobals.cropPath = "";
            hruGlobals.soilPath = "";
            hruGlobals.LanduseTableName = "";
            hruGlobals.SoilTableName = "";
            hruGlobals.useSSURGO = false;
            //If useSSURGO Then
            //    selectSoilTable.Visible = False
            //    soilLabel.Visible = False
            //End If
            hruGlobals.cellArea = 0.0;

            string cropDatPath = "";
            string fertDatPath = "";
            string pestDatPath = "";
            string tillDatPath = "";
            string urbanDatPath = "";
            string cropDatDestPath = "";
            string fertDatDestPath = "";
            string pestDatDestPath = "";
            string tillDatDestPath = "";
            string urbanDatDestPath = "";

            cropDatPath = hruGlobals.origPath + @"\Databases\crop.dat";
            cropDatDestPath = hruGlobals.TxtInOutDir + @"\crop.dat";

            fertDatPath = hruGlobals.origPath + @"\Databases\fert.dat";
            pestDatPath = hruGlobals.origPath + @"\Databases\pest.dat";
            tillDatPath = hruGlobals.origPath + @"\Databases\till.dat";
            urbanDatPath = hruGlobals.origPath + @"\Databases\urban.dat";

            fertDatDestPath = hruGlobals.TxtInOutDir + @"\fert.dat";
            pestDatDestPath = hruGlobals.TxtInOutDir + @"\pest.dat";
            tillDatDestPath = hruGlobals.TxtInOutDir + @"\till.dat";
            urbanDatDestPath = hruGlobals.TxtInOutDir + @"\urban.dat";

            string septwqDatPath = "";
            string septwqDatDestPath = "";
            septwqDatPath = hruGlobals.origPath + @"\Databases\septwq.dat";
            septwqDatDestPath = hruGlobals.TxtInOutDir + @"\septwq.dat";

            string plantDatPath = hruGlobals.origPath + @"\Databases\plant.dat";
            string plantDatDestPath = hruGlobals.TxtInOutDir + @"\plant.dat";

            if (mwSWATDbUtils.hasData(hruGlobals, "BASINSDATA1") &&
                  hruGlobals.cropPath.Equals("") == false &&
                  System.IO.File.Exists(hruGlobals.cropPath) &&
                  hruGlobals.soilPath.Equals("") == false &&
                  System.IO.File.Exists(hruGlobals.soilPath))
            {
                //This argument is missing. HK
            }

            if (System.IO.File.Exists(cropDatDestPath) == false)
            {
                if (System.IO.File.Exists(cropDatPath))
                {
                    System.IO.File.Copy(cropDatPath, cropDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + cropDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }
            if (System.IO.File.Exists(fertDatDestPath) == false)
            {
                if (System.IO.File.Exists(fertDatPath))
                {
                    System.IO.File.Copy(fertDatPath, fertDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + fertDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }
            if (System.IO.File.Exists(pestDatDestPath) == false)
            {
                if (System.IO.File.Exists(pestDatPath))
                {
                    System.IO.File.Copy(pestDatPath, pestDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + pestDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }
            if (System.IO.File.Exists(plantDatDestPath) == false)
            {
                if (System.IO.File.Exists(plantDatPath))
                {
                    System.IO.File.Copy(plantDatPath, plantDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + plantDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }

            if (System.IO.File.Exists(septwqDatDestPath) == false)
            {
                if (System.IO.File.Exists(septwqDatPath))
                {
                    System.IO.File.Copy(septwqDatPath, septwqDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + septwqDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }

            if (System.IO.File.Exists(tillDatDestPath) == false)
            {
                if (System.IO.File.Exists(tillDatPath))
                {
                    System.IO.File.Copy(tillDatPath, tillDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + tillDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }

            if (System.IO.File.Exists(urbanDatDestPath) == false)
            {
                if (System.IO.File.Exists(urbanDatPath))
                {
                    System.IO.File.Copy(urbanDatPath, urbanDatDestPath);
                }
                else
                {
                    //MessageBox.Show(strings_vb.nodatafile + urbanDatPath, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                }
            }


        }

        private string watershedShapefile = "";
        private string streamShapefile = "";
        private string outletsInletsShapefile = "";

        private MapWinGIS.Shapefile wshd = null;
        private MapWinGIS.Shapefile net = null;
        private MapWinGIS.Shapefile oiFile = null;
        private MapWinGIS.Shapefile extraOiFile = null;
        public bool LabelBasins(string watershedShpPath, string streamNetShpPath, string oiShpPath, string extraOiShpPath)
        {
            SetProjection();

            
            MapWinGIS.Grid dem = new MapWinGIS.Grid(); dem.Open(basedem);
            //MapWindow.Interfaces.Layer demLayer = (MapWindow.Interfaces.Layer)dem;

            wshd = new MapWinGIS.Shapefile(); wshd.Open(watershedShpPath);
            //MapWindow.Interfaces.Layer wshdLayer = (MapWindow.Interfaces.Layer)wshd;

            net = new MapWinGIS.Shapefile(); net.Open(streamNetShpPath);
           // MapWindow.Interfaces.Layer netLayer = (MapWindow.Interfaces.Layer)net;

            

            oiFile = new MapWinGIS.Shapefile(); oiFile.Open(oiShpPath);
            //MapWindow.Interfaces.Layer oiFileLayer = (MapWindow.Interfaces.Layer)oiFile;

            if (extraOiShpPath != "")
            {
                extraOiFile = new MapWinGIS.Shapefile();
                extraOiFile.Open(extraOiShpPath);
            }
            //MapWindow.Interfaces.Layer extraOiFileLayer = (MapWindow.Interfaces.Layer)extraOiFile;

            //hruGlobals.populateLinkToBasin(demLayer, wshdLayer, netLayer, oiFileLayer, extraOiFileLayer); //Passing, exception,, cannot convert UTM to LatLong
            hruGlobals.populateLinkToBasin(dem, wshd, net, oiFile, extraOiFile); //Passing, exception,, cannot convert UTM to LatLong

            hruGlobals.clearCentroids();
            int numFields = wshd.NumFields;
            // Find index for PolygonID field
            int polyidField = -1;
            for (int i = 0; i < numFields; i++)
            {
                if (wshd.Field[i].Name.Equals("PolygonID", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    polyidField = i;
                    break;
                }
            }
            //////////////////////////////////////////////////////////////////////////
            int numShapes = wshd.NumShapes;
            for (int i = 0; i < numShapes; i++)
            {
                MapWinGIS.Shape shape = wshd.Shape[i];
                MapWinGIS.Point centroid = MapWinGeoProc.Utils.Centroid(ref shape);
                int link = Convert.ToInt32(wshd.CellValue[polyidField, i]);
                if (hruGlobals.linkToBasin.ContainsKey(link) == false)
                {
                    //MessageBox.Show(strings_vb.nobasinforlink & link.ToString() & _
                    //strings_vb.tryrestart, MWSWATName, _
                    //MessageBoxButtons.OK, MessageBoxIcon.Error)
                    return false;
                }

                int basin = hruGlobals.linkToBasin[link];
                hruGlobals.setCentroid(basin, centroid);
                if (hruGlobals.basinToSWATBasin.ContainsKey(basin))
                {
                    int SWATBasin = hruGlobals.basinToSWATBasin[basin];
                    //wshdLayer.AddLabel(SWATBasin.ToString(), System.Drawing.Color.Black, centroid.x, centroid.y, MapWinGIS.tkHJustification.hjCenter)
                }
            }
            return true;
        }

        public bool SetProjection()
        {
            //HK: Should be updated. In my simulation, just using predefined projection.
            string prjFilePath = System.IO.Path.GetDirectoryName(basedem) + @"\" + System.IO.Path.GetFileNameWithoutExtension(basedem) + @".prj";
            System.IO.TextReader prjFile = new System.IO.StreamReader(prjFilePath);
            string proj = prjFile.ReadLine();
            prjFile.Close();
            int idxleft = proj.IndexOf("Zone");
            int idxright = proj.IndexOf('"', idxleft + 5);
            string Zone = proj.Substring(idxleft + 5, idxright - (idxleft + 5));
            Zone = "14";
            Zone = "48";
            //string Zone = "14"; //??? is it for only this example?
            hruGlobals.utmProjection = "+proj=utm +zone=" + Zone + " +ellps=WGS84 +towgs84=0,0,0,0,0,0,0 +datum=WGS84";
            //hruGlobals.utmProjection = "+proj = utm + zone = " + Zone + " + ellps = WGS84 + datum = WGS84 + units = m + no_defs";
            
            //hruGlobals.utmProjection = "+proj=utm +zone=14 +ellps=WGS84 +datum=WGS84 +units=m +no_defs";
            //hruGlobals.utmProjection = Zone;
            //hruGlobals.utmProjection = "+proj = latlong + ellps = WGS84 + to_meter = 1.0000000000";
            return true;
        }

        private MapWinGIS.Grid landuseLayer = null;
        public bool SetCropPath(string cropPath)
        {
            string cropFileName = System.IO.Path.GetFileName(cropPath);
            string temp = prjPath + @"\Source\crop\" + cropFileName;
            if (System.IO.File.Exists(temp) == false)
            {
                System.IO.File.Copy(cropPath, temp);
            }
            // copy .prj file if any
            string prjsource = System.IO.Path.ChangeExtension(cropFileName, ".prj");
            string prjtarget = System.IO.Path.ChangeExtension(temp, ".prj");
            if (System.IO.File.Exists(prjsource) && (!System.IO.File.Exists(prjtarget)))
            {
                System.IO.File.Copy(prjsource, prjtarget);
            }
            hruGlobals.cropPath = temp;
            landuseLayer = new MapWinGIS.Grid();
            landuseLayer.Open(cropPath);
            return true;
        }

        private MapWinGIS.Grid soilLayer = null;
        public bool SetSoilPath(string soilPath)
        {
            string soilFileName = System.IO.Path.GetFileName(soilPath);
            string temp = prjPath + @"\Source\soil\" + soilFileName;
            if (System.IO.File.Exists(temp) == false)
            {
                System.IO.File.Copy(soilPath, temp);
            }
            // copy .prj file if any
            string prjsource = System.IO.Path.ChangeExtension(soilFileName, ".prj");
            string prjtarget = System.IO.Path.ChangeExtension(temp, ".prj");
            if (System.IO.File.Exists(prjsource) && (!System.IO.File.Exists(prjtarget)))
            {
                System.IO.File.Copy(prjsource, prjtarget);
            }
            hruGlobals.soilPath = temp;
            soilLayer = new MapWinGIS.Grid();
            soilLayer.Open(soilPath);
            return true;
        }

        public bool SetLanduseTable(string landuseTableName)
        {
            hruGlobals.LanduseTableName = landuseTableName;
            mwSWATDbUtils.InitLanduses(hruGlobals);
            return true;
        }

        public bool SetSoilTablePath(string soilTablePath)
        {
            hruGlobals.SoilTableName = soilTablePath;
            //hruGlobals.UsersoilTableName = usersoilTableName
            //soilTableName = hruGlobals.SoilTableName
            mwSWATDbUtils.InitSoils(hruGlobals);
            return true;
        }

        public bool SetSlopeLimits(string slopeLimits)
        {
            hruGlobals.parseLimits(slopeLimits);
            return true;
        }

        public bool ReadBasinData(bool fullHruWanted)
        {
            hruData.generateBasins(landuseLayer, soilLayer, net, fullHruWanted, hruGlobals); //Error occured when the study area is small


            //MapWinGIS.Shapefile fullHrusShp = new MapWinGIS.Shapefile();
            //fullHrusShp.Open(hruGlobals.fullHRUsPath);

            //hruData.printBasins(hruGlobals, false, fullHrusShp);
            return true;
        }

        public bool CreateHru(double minCrop, double minSoil, double minSlope)
        {
            hruGlobals.singleOrMultiple = CreateHRU.SingleOrMultiple.multipleHRUs;
            hruGlobals.byAreaOrPercent = CreateHRU.ByAreaOrPercent.byPercent;

            double minCropPercent = hruData.minMaxCropPercent();
            // recmd_crop used as limit check
            // use it at 99.9% to avoid dangers of rounding errors
            double recmd_crop = 0.999 * minCropPercent;
            hruGlobals.minCropPercent = recmd_crop;

            double minSoilPercent = hruData.minMaxSoilPercent(hruGlobals.minCropPercent);
            double recmd_soil = 0.999 * minSoilPercent;
            hruGlobals.minSoilPercent = recmd_soil;

            double minSlopePercent = hruData.minMaxSlopePercent(hruGlobals.minCropPercent, hruGlobals.minSoilPercent);
            double recmd_slope = 0.999 * minSlopePercent;
            hruGlobals.minSlopePercent = recmd_slope;

            if (hruGlobals.singleOrMultiple == SingleOrMultiple.multipleHRUs)
            {
                if (hruGlobals.byAreaOrPercent == ByAreaOrPercent.byArea)
                {
                    try
                    {
                        //hruGlobals.minArea = CDbl(Me.Area.Text);
                    }
                    catch
                    {
                        //MessageBox.Show(strings_vb.selectarealimit, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        return false;
                    }
                    // remove hrus below thresholds
                    hruData.removeSmallHRUsByArea(hruGlobals);
                }
                else if (hruGlobals.byAreaOrPercent == ByAreaOrPercent.byPercent)
                {
                    try
                    {
                        hruGlobals.minCropPercent = minCrop;
                    }
                    catch
                    {
                        //MessageBox.Show(strings_vb.selectlandusethreshold, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        return false;
                    }
                    if (hruGlobals.minCropPercent > recmd_crop)
                        hruGlobals.minCropPercent = recmd_crop;

                    try
                    {
                        hruGlobals.minSoilPercent = minSoil;
                    }
                    catch
                    {
                        //MessageBox.Show(strings_vb.selectsoilthreshold, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        return false;
                    }
                    if (hruGlobals.minSoilPercent > recmd_soil)
                        hruGlobals.minSoilPercent = recmd_soil;

                    try
                    {
                        hruGlobals.minSlopePercent = minSlope;
                    }
                    catch
                    {
                        //MessageBox.Show(strings_vb.selectslopethreshold, "MWSWAT", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        //Exit Sub
                        return false;
                    }
                    if (hruGlobals.minSlopePercent > recmd_slope)
                        hruGlobals.minSlopePercent = recmd_slope;


                    if (hruData.cropSoilAndSlopePercentsAreOK(hruGlobals) == false)
                    {
                        throw (new Exception("thresholdtoohigh"));  // Should not happen!
                    }
                    // remove hrus below thresholds
                    hruData.removeSmallHRUsByPercent(hruGlobals);
                }
                hruData.splitHrus(hruGlobals);
            }
            else
            {
                throw (new Exception(""));
            }
            hruData.basinsToHRUs(hruGlobals);


            //MapWinGIS.Shapefile fullHrusShp = new MapWinGIS.Shapefile();
            //fullHrusShp.Open(hruGlobals.fullHRUsPath);
            //hruData.printBasins(hruGlobals, true, fullHrusShp);
            return true;
        }
    }
}
