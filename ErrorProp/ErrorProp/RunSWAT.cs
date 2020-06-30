using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CreateHRU;
using System.Collections;
using System.IO;

namespace HydroResponseUnitGenerator
{

    public class RunSWAT
    {
        private mwSWATGlobals hruGlobals;
        private HRUs hruData;
        private string prjPath;

        public RunSWAT(HRUs data, mwSWATGlobals globals, string path)
        {
            hruGlobals = globals;
            hruData = data;
            prjPath = path;
        }

        public bool Init()
        {
            hruGlobals.dailyPrecipitation = true;
            hruGlobals.IDT = 0;

            hruGlobals.weatherSource = mwSWATDbUtils.StringToWeatherSource("file");
            hruGlobals.weatherStationsTable = "";
            hruGlobals.weatherStationListPath = "";
            hruGlobals.wgenSource = mwSWATDbUtils.StringToWgenSource("file");
            hruGlobals.wgenFile = "";
            hruGlobals.weatherGeneratorTable = "";
            return true;
        }

        private int startDay = 0;
        private int startMonth = 0;
        private int startYear = 0;
        private int finishDay = 0;
        private int finishMonth = 0;
        private int finishYear = 0;
        public bool SetTimeSpan(int startYr, int startMth, int startDy, int finishYr, int finishMth, int finishDy)
        {
            startDay = startDy;
            startMonth = startMth;
            startYear = startYr;
            finishDay = finishDy;
            finishMonth = finishMth;
            finishYear = finishYr;
            return true;
        }

        public bool SetWeatherInfo(string weatherStationFile, string weatherGeneratorFile, bool isGlobalFile, string baseDemPath)
        {
            hruGlobals.weatherStationListPath = weatherStationFile;
            hruGlobals.wgenFile = weatherGeneratorFile;
            if (isGlobalFile == false)
                hruGlobals.weatherSource = CreateHRU.WeatherSource.file;
            else
                hruGlobals.weatherSource = CreateHRU.WeatherSource.raw;

            hruGlobals.wgenSource = CreateHRU.WgenSource.file;

            string sourceFolder = prjPath + @"\Source";
            string weatherShapeFile = CreateHRU.mwSWATDbUtils.InitWeather(sourceFolder, true, hruData, hruGlobals); //Not running
            string basePrj = sourceFolder + @"\" + System.IO.Path.GetFileNameWithoutExtension(baseDemPath) + ".prj";
            string shapePrj = sourceFolder + @"\" + System.IO.Path.GetFileNameWithoutExtension(weatherShapeFile) + ".prj";
            if (System.IO.File.Exists(basePrj))
                System.IO.File.Copy(basePrj, shapePrj, true);

            return true;
        }

        public bool WriteFiles(int rrrType, int rdType, double exponen, int petType, int pfType, string streamShpPath)
        {

            hruGlobals.NBYR = finishYear - startYear + 1;
            hruGlobals.IYR = startYear;
            hruGlobals.IDAF = mwSWATDbUtils.toJulianDay(startDay, startMonth, startYear);
            hruGlobals.IDAL = mwSWATDbUtils.toJulianDay(finishDay, finishMonth, finishYear);

            hruGlobals.IEVENT = rrrType;
            hruGlobals.IDIST = rdType;
            if (rdType == 1)
                hruGlobals.REXP = exponen;

            hruGlobals.IPET = petType;
            hruGlobals.IPRINT = pfType;

            //////////////////////////////////////////////////////////////////////////
            hruGlobals.weatherWanted = true;
            hruGlobals.bsnTableWanted = true;
            hruGlobals.chmTableWanted = true;
            hruGlobals.cioTableWanted = true;
            hruGlobals.figFileWanted = true;
            hruGlobals.gwTableWanted = true;
            hruGlobals.hruTableWanted = true;
            hruGlobals.mgtTableWanted = true;
            hruGlobals.pndTableWanted = true;
            hruGlobals.ppTableWanted = true;
            hruGlobals.ppiTableWanted = true;
            hruGlobals.resTableWanted = true;
            hruGlobals.rteTableWanted = true;
            hruGlobals.sepTableWanted = true;
            hruGlobals.solTableWanted = true;
            hruGlobals.subTableWanted = true;
            hruGlobals.swqTableWanted = true;
            hruGlobals.wgnTableWanted = true;
            hruGlobals.wusTableWanted = true;
            hruGlobals.wwqTableWanted = true;

            bool figOK = topology(streamShpPath);
            mwSWATDbUtils.WriteDb(hruData, hruGlobals, figOK);
            //mwSWATDbUtils.WriteDb();
            return true;
        }

        public bool topology(string streamsLayer)
        {
            MapWinGIS.Shapefile streamShp = new MapWinGIS.Shapefile();
            streamShp.Open(streamsLayer);
                        
            ArrayList array = mwSWATTopology.readTree(hruGlobals, streamShp);

            hruData.computeDrainAreas(array, hruGlobals);

            TopologyData cd = new TopologyData();

            // add internal outlets (monitoring points)
            int monNum = 1;
            for (int i = 0; i < hruGlobals.monitoringPointBasins.Count; i++)
            {
                int basin = hruGlobals.monitoringPointBasins[i];
                // guard against basins upstream from inlets
                if (hruGlobals.basinToSWATBasin.ContainsKey(basin))
                {
                    int SWATBasin = hruGlobals.basinToSWATBasin[basin];
                    cd.addSavePoint(basin, SWATBasin, monNum);
                    monNum = monNum + 1;
                }
            }
            // add reservoirs
            int resNum = 1;
            for (int i = 0; i < hruGlobals.reservoirBasins.Count; i++)
            {
                int basin = hruGlobals.reservoirBasins[i];
                // guard against basins upstream from inlets
                if (hruGlobals.basinToSWATBasin.ContainsKey(basin))
                {
                    int SWATBasin = hruGlobals.basinToSWATBasin[basin];
                    cd.addReservoir(basin, SWATBasin, resNum);
                    resNum = resNum + 1;
                }
            }
            // add inlets
            int fileNum = 1;
            for (int i = 0; i < hruGlobals.inletBasins.Count; i++)
            {
                int basin = hruGlobals.inletBasins[i];
                // guard against inlets upstream from inlets (!)
                if (hruGlobals.basinToSWATBasin.ContainsKey(basin))
                {
                    int SWATBasin = hruGlobals.basinToSWATBasin[basin];
                    cd.addInlet(basin, SWATBasin, fileNum);
                    fileNum = fileNum + 1;
                }
            }
            // add point sources (still incrementing file numbers)
            for (int i = 0; i < hruGlobals.srcBasins.Count; i++)
            {
                int basin = hruGlobals.srcBasins[i];
                // guard against basins upstream from inlets
                if (hruGlobals.basinToSWATBasin.ContainsKey(basin))
                {
                    int SWATBasin = hruGlobals.basinToSWATBasin[basin];
                    cd.addPointSource(basin, SWATBasin, fileNum);
                    fileNum = fileNum + 1;
                }
            }
            if (hruGlobals.figFileWanted)
            {
                // Write fig.fig file
                string configFilePath = hruGlobals.TxtInOutDir + "\\fig.fig";
                return mwSWATTopology.makeConfigFile(array, cd, configFilePath, hruGlobals);
            }

            return true;
        }

        public bool Run(bool debugMode, bool is64)
        {
            Process process = new Process
            {
                StartInfo = { CreateNoWindow = false, WorkingDirectory = this.hruGlobals.TxtInOutDir, FileName = this.hruGlobals.origPath + @"\runswat.bat" }
            };
            string destFileName = this.hruGlobals.origPath + @"\swat2012.exe";
            string str = this.hruGlobals.origPath + @"\SWAT";
            if (debugMode)
            {
                if (is64)
                {
                    this.hruGlobals.swatPath = str + "_64debug.exe";
                }
                else
                {
                    this.hruGlobals.swatPath = str + "_32debug.exe";
                }
            }
            else if (is64)
            {
                this.hruGlobals.swatPath = str + "_64rel.exe";
            }
            else
            {
                this.hruGlobals.swatPath = str + "_32rel.exe";
            }
            File.Copy(this.hruGlobals.swatPath, destFileName, true);
            process.StartInfo.Arguments = "\"" + destFileName + "\"";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                return true;
            }
            return false;
        }

    }
}
