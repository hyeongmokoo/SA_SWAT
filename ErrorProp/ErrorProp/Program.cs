using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WatershedDelienation;
using HydroResponseUnitGenerator;
using System.IO;

namespace ErrorProp
{
    class Program
    {


        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            string file_save_path = @"C:\SWAT\ProgTest\";
            string prjName = "xgeswat"; //Using Just default name
            string saveName = file_save_path + prjName;
            string prjDir = System.IO.Path.GetDirectoryName(saveName);



            WaterDelineation wd = new WaterDelineation();
            //string input_dem_path = "";
            //string filled_dem_path = "";
            //string mask_shape_path = "";
            //string masked_dem_path = "";
            //string d8_path = "";
            //string d8_slope_path = "";
            string dinf_path = "";
            //string dinf_slope_path = "";
            string area_d8_path = "";
            string area_dinf_path = "";
            //string snaped_outlet_path = "";

            string StrahlOrdResultPath = "";
            string LongestUpslopeResultPath = "";
            string TotalUpslopeResultPath = "";
            string StreamGridResultPath = "";
            string StreamOrdResultPath = "";
            string TreeDatResultPath = "";
            string CoordDatResultPath = "";
            //string StreamShapeResultPath = "";
            //string WatershedGridResultPath = "";

            //string WatershedShapePath = "";
            string MergedWatershedShpPath = "";
            string shape_outlet = "";

            //Copying result file to data folder, Do not need this process
            string input_dem_path = saveName + @"\Source\dem.tif";//Input 1
            //string input_dem_path = saveName + @"\Source\srtm_57_06.tif";//Input 1 ##Test scale 
            string originalDem = saveName + @"\Source\dem_Fill.tif"; 
            string landuseGrid = saveName + @"\Source\landuse.tif"; //Manually copied in here //Inputfile 3
            string soilGrid = saveName + @"\Source\soil.tif";//Manually copied in here //Inputfile 4

            string d8 = saveName + @"\Source\d8.tif";
            string d8Slope = saveName + @"\Source\d8slope.tif";
            string dInfSlope = saveName + @"\Source\dInfSlope.tif";
            string watershedGridResultPath = saveName + @"\Source\WatershedGridResult.tif";

            string watershedShpPath = saveName + @"\Source\watershedShp.shp";
            string streamNetShpPath = saveName + @"\Source\StreamShapeResult.shp";
            string outletShpPath = saveName + @"\Source\shape_outlet.shp"; 

            //string fullHRUs = saveName + @"\Source\FullHRUs.shp";
            string weatherStationInfo = saveName + @"\Source\stations.txt"; //Inputfile 5
            string weatherGeneratorInfo = saveName + @"\Source\generator.wgn"; //Inputfile 6
            //string pcpInfo = saveName + @"\Source\765850.pcp"; //Inputfile 7
            //string tmpInfo = saveName + @"\Source\765850.tmp"; //Inputfile 8


            CreateHRU.mwSWATGlobals hruGlobals = new CreateHRU.mwSWATGlobals("");
            CreateHRU.HRUs hruData = new CreateHRU.HRUs();
            HRU hru = null;

            Console.WriteLine("DEM processing");
            //DEM Processing
             
            //string mask_shape_path = file_save_path + "mask.shp";
           // masked_dem_path = file_save_path + "MaskedDEM.tif";

            //wd.RunMask(input_dem_path, mask_shape_path, masked_dem_path); //Mask, this is not required. HK. Using Map windows


            //filled_dem_path = file_save_path + "FilledDEM.tif";

            //Console.WriteLine("Fill DEM");
            wd.RunPitFill(input_dem_path, originalDem);
            //wd.RunPitFill(masked_dem_path, filled_dem_path); // This might be required. HK.


            //Console.WriteLine("D8");
            //d8_path = file_save_path + "d8.tif";
            //d8_slope_path = file_save_path + "d8slope.tif";

            wd.RunD8(originalDem, d8, d8Slope); 


            //Console.WriteLine("DInfSlope");
            dinf_path = saveName + @"\Source\dInf.tif";
            //dinf_slope_path = file_save_path + "dInfslope.tif";

            wd.RunDInf(originalDem, dinf_path, dInfSlope); //Slope.

            //Console.WriteLine("AreaD8");
            area_d8_path = saveName + @"\Source\AreaD8.tif";

            wd.RunAreaD8(d8, area_d8_path); //Area D8


            //Console.WriteLine("AreaDInf");
            area_dinf_path = saveName + @"\Source\AreaDInf.tif";

            wd.RunAreaDInf(dinf_path, area_dinf_path);


            //Console.WriteLine("OrignalStreamShape");
            string strDelenationThreshold = saveName + @"\Source\threshold.txt"; ///P2!!
            int intThreshold = Convert.ToInt32(System.IO.File.ReadAllLines(strDelenationThreshold)[0]);
            int deleneation_threshold = intThreshold; //It can be Changed, by determining as a parameter
            //int deleneation_threshold = 5000;

            StrahlOrdResultPath = saveName + @"\Source\StrahlOrdResult.tif";
            LongestUpslopeResultPath = saveName + @"\Source\LongestUpslopeResult.tif";
            TotalUpslopeResultPath = saveName + @"\Source\TotalUpslopeResult.tif";
            StreamGridResultPath = saveName + @"\Source\StreamGridResult.tif";
            StreamOrdResultPath = saveName + @"\Source\StreamOrdResult.tif";
            TreeDatResultPath = saveName + @"\Source\TreeDatResult.dat";
            CoordDatResultPath = saveName + @"\Source\CoordDatResult.dat";
            //StreamShapeResultPath = file_save_path + @"\Source\StreamShapeResult.shp";
            //WatershedGridResultPath = file_save_path + @"\Source\WatershedGridResult.tif";

            wd.RunDefineStreamGrids(deleneation_threshold,
                input_dem_path, originalDem,
                d8, d8Slope,
                area_d8_path, area_dinf_path,
                StrahlOrdResultPath,
                LongestUpslopeResultPath,
                TotalUpslopeResultPath,
                StreamGridResultPath,
                StreamOrdResultPath,
                TreeDatResultPath,
                CoordDatResultPath,
                streamNetShpPath,
                watershedGridResultPath); //Stream with values..

            //Outlet..? Do we need outlet shps?
            //Console.WriteLine("Snapping Outlets");
            string original_outlet_shape_path = saveName + @"\Source\outlet.shp"; //Inputfile 2
            //snaped_outlet_path = file_save_path + "SnapeOutlet.shp";
            double threshold = 300;

            wd.RunAutoSnap(original_outlet_shape_path, outletShpPath, d8, streamNetShpPath, threshold); //Outlet snaps.


            //Console.WriteLine("WatershedGrid");

            //StrahlOrdResultPath = saveName + @"\Source\StrahlOrdResult.tif";
            //LongestUpslopeResultPath = saveName + @"\Source\LongestUpslopeResult.tif";
            //TotalUpslopeResultPath = saveName + @"\Source\TotalUpslopeResult.tif";
            //StreamGridResultPath = saveName + @"\Source\StreamGridResult.tif";
            //StreamOrdResultPath = saveName + @"\Source\StreamOrdResult.tif";
            //TreeDatResultPath = saveName + @"\Source\TreeDatResult.dat";
            //CoordDatResultPath = saveName + @"\Source\CoordDatResult.dat";
            //streamNetShpPath = saveName + @"\Source\StreamShapeResult.shp";
            //watershedGridResultPath = saveName + @"\Source\WatershedGridResult.tif";

            //wd.RunDefineStreamGrids(deleneation_threshold, input_dem_path, originalDem, d8, d8Slope,
            //    area_d8_path, area_dinf_path, outletShpPath, StrahlOrdResultPath, LongestUpslopeResultPath, TotalUpslopeResultPath,
            //    StreamGridResultPath, StreamOrdResultPath, TreeDatResultPath, CoordDatResultPath, streamNetShpPath,
            //    watershedGridResultPath); //Watershed grid


            //Console.WriteLine("WatershedShape"); //This process requires much calculation time

            //WatershedShapePath = file_save_path + "watershedShp.shp";
            MergedWatershedShpPath = saveName + @"\Source\watershedShp_merged.shp";
            shape_outlet = saveName + @"\Source\shape_outlet.shp";

            wd.RunWshedToShape(d8, watershedGridResultPath, watershedShpPath);
            wd.RunApplyStreamAttributes(streamNetShpPath, originalDem, watershedShpPath); //change to filled dem from origianl
            wd.RunApplyWatershedAttributes(watershedShpPath, streamNetShpPath);
            bool ret = wd.RunBuildJoinedBasins(watershedShpPath, shape_outlet, MergedWatershedShpPath);


            //Console.WriteLine("HydroResponseUnitState");
            
            //Checking this part //Delete all outputs??
            //string delDir = prjDir;
            //DeleteFiles(delDir + "\\");

            //if (!prjDir.EndsWith("\\"))
            //    prjDir += "\\";

            string prjPath = saveName;

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //Start to generati
            string dbPath;
            //string soilDir;
            //string cropDir;
            //Create This folder befor simulation
            hruGlobals.TxtInOutDir = prjPath + @"\Scenarios\Default\TxtInOut";
            //soilDir = prjPath + @"\Source\soil"; //Why this need?
            //cropDir = prjPath + @"\Source\crop";
            dbPath = hruGlobals.origPath + @"\vgeswat2012.mdb";

            //Change this.
            //hruGlobals.dbPath = prjPath + "\\" + prjName + ".mdb";
            hruGlobals.dbPath = dbPath; //Changed to DB path
            //if (!System.IO.File.Exists(hruGlobals.dbPath))
            //{
            //    if (System.IO.File.Exists(dbPath))
            //    {
            //        System.IO.File.Copy(dbPath, hruGlobals.dbPath);
            //    }
            //}

            bool dailyPrecipitation = true;
            hruGlobals.dailyPrecipitation = dailyPrecipitation;

            hru = new HRU(hruData, hruGlobals, prjPath);

            
            ////Copy from A to B (A, B)
            //CopyFile(filled_dem_path, originalDem);
            //CopyFile(d8_path, d8);
            //CopyFile(d8_slope_path, d8Slope);
            //CopyFile(dinf_slope_path, dInfSlope);
            //CopyFile(WatershedGridResultPath, watershedGridResultPath);
            //CopyFile(WatershedShapePath, watershedShpPath);
            //CopyFile(StreamShapeResultPath, streamNetShpPath);
            //CopyFile(snaped_outlet_path, outletShpPath);


            Console.WriteLine("Parameters for HRU");

            //Min percents for each categories
            string strLuMinpath = saveName + @"\Source\lumindef.txt"; //P5
            double dblLuMinDef = Convert.ToDouble(System.IO.File.ReadAllLines(strLuMinpath)[0]);
            double minCrop = dblLuMinDef;

            string strSoMinPath = saveName + @"\Source\somindef.txt"; //P6
            double dblSoMinDef = Convert.ToDouble(System.IO.File.ReadAllLines(strSoMinPath)[0]);
            double minSoil = dblSoMinDef;

            string strSlMinPath = saveName + @"\Source\slmindef.txt"; //P7
            double dblSlMinDef = Convert.ToDouble(System.IO.File.ReadAllLines(strSlMinPath)[0]);
            double minSlope = dblSlMinDef;

            //double minCrop = 20, minSoil = 10, minSlope = 5;
            int year_start = 2013, month_start = 1, day_start = 1;
            int year_end = 2013, month_end = 12, day_end = 31;

            //Pass slope limitation and rainfall runoff, rainfall distribution, Potentail_ET_Method; Setting as default
            List<int> slope_limitation = new List<int>();
            
            int rainfall_runoff_routine = 0;
            //int rainfall_dsitribution = 0;
            int potentail_ET = 1;
            int printout_frequently = 0;

            hru.Init(originalDem, watershedGridResultPath, dInfSlope, d8Slope, d8);
            hru.LabelBasins(watershedShpPath, streamNetShpPath, outletShpPath, "");
            hru.SetCropPath(landuseGrid);
            hru.SetSoilPath(soilGrid);
            hru.SetLanduseTable("global_landuses");
            hru.SetSoilTablePath("global_soils");

            //Add slope limitation from a text file
            string strSlopeTxt = saveName + @"\Source\slopdef.txt"; //P8
            int intSlopeDef = Convert.ToInt32(System.IO.File.ReadAllLines(strSlopeTxt)[0]);
            slope_limitation.Add(intSlopeDef); //Input 9

            string slope_limits_str = "[0,";
            for (int iSlope = 0; iSlope < slope_limitation.Count; iSlope++)
            {
                slope_limits_str += slope_limitation[iSlope].ToString() + ",";
            }
            slope_limits_str += "9999]";
            hru.SetSlopeLimits(slope_limits_str);
            hru.ReadBasinData(false);
            //hru.ReadBasinData(false);

            hru.CreateHru(minCrop, minSoil, minSlope);

            Console.WriteLine("Writing files");
            RunSWAT runSwat = new RunSWAT(hruData, hruGlobals, prjPath);
            runSwat.SetTimeSpan(year_start, month_start, day_start, year_end, month_end, day_end);
            //runSwat.SetWeatherInfo(weatherStationInfo, weatherGeneratorInfo, false, originalDem);
            runSwat.SetWeatherInfo(weatherStationInfo, weatherGeneratorInfo, false, input_dem_path);
            runSwat.WriteFiles(rainfall_runoff_routine, 0, 0, potentail_ET, printout_frequently, streamNetShpPath);
            Console.WriteLine("Complete");

        }

        static void DeleteFiles(string dir)
        {
            DirectoryInfo di = new DirectoryInfo(dir);
            if (di.GetDirectories().Length == 0 && di.GetFiles().Length == 0) return;
            foreach (DirectoryInfo d in di.GetDirectories())
                DeleteFiles(d.FullName);
            foreach (FileInfo fi in di.GetFiles())
                fi.Delete();
            di.Delete();
        }

        static void CopyFile(string file_name, string new_file_name)
        {
            string new_file_name_without_ext = Path.GetFileNameWithoutExtension(new_file_name);
            string file_name_without_ext = Path.GetFileNameWithoutExtension(file_name);
            string file_dir = Directory.GetParent(file_name).FullName;
            string[] file_list = Directory.GetFiles(file_dir, file_name_without_ext + ".*");
            for (int iFile = 0; iFile < file_list.Length; iFile++)
            {
                string temp_file_name = file_list[iFile];
                string temp_new_file_name = Directory.GetParent(new_file_name) + "\\" +
                    new_file_name_without_ext + Path.GetExtension(temp_file_name);
                File.Copy(temp_file_name, temp_new_file_name, true);
            }
        }
    }
}
