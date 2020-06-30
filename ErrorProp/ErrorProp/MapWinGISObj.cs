using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WatershedDelienation
{
    public class CallbackClient : MapWinGIS.ICallback
    {
        public void Error(string KeyOfSender, string ErrorMsg)
        {
            int a = 0;
            //throw new NotImplementedException();
        }

        public void Progress(string KeyOfSender, int Percent, string Message)
        {
            int b = 0;
            //throw new NotImplementedException();
        }
    }

    public class WaterDelineation
    {
        public WaterDelineation()
        {

        }

        public bool RunMask(string demPath, string maskPath, string resultPath)
        {
            CallbackClient client = new CallbackClient();

            ArrayList selectedIndex = new ArrayList();
            selectedIndex.Add(0);
            int ret = MapWinGeoProc.Hydrology.Mask(demPath, maskPath, selectedIndex, resultPath, client);
            if (ret == 0) return true;
            return false;
        }

        public bool RunPitFill(string srcFile, string destFile)
        {
            CallbackClient client = new CallbackClient();
            try
            {
                MapWinGeoProc.Hydrology.Fill(srcFile, destFile, client);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        public bool RunBurnIn(string strBurn, string strDEM, string strBurnResult)
        {
            CallbackClient client = new CallbackClient();
            int ret = MapWinGeoProc.Hydrology.CanyonBurnin(strBurn, strDEM, strBurnResult, client);
            if (ret == 0) return true;
            return false;
        }

        public bool RunD8(string pitFillPath, string d8ResultPath, string d8SlopeResultPath)
        {
            CallbackClient client = new CallbackClient();
            int ret = MapWinGeoProc.Hydrology.D8(pitFillPath, d8ResultPath, d8SlopeResultPath, 8, false, client);
            if (ret == 0) return true;
            return false;
        }

        public bool RunDInf(string pitFillPath, string dInfResultPath, string dInfSlopeResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.DInf(pitFillPath, dInfResultPath, dInfSlopeResultPath, 8, false, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunAreaD8(string d8Path, string ad8ResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.AreaD8(d8Path, "", ad8ResultPath, false, false, 8, false, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunAreaDInf(string dInfPath, string adInfResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.AreaDInf(dInfPath, "", adInfResultPath, false, false, 8, false, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunGridNetwork(string demPathWithMask,
            string d8Path, string StrahlOrdResultPath,
            string LongestUpslopeResultPath,
            string TotalUpslopeResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.RunGridNetwork(demPathWithMask, d8Path,
                LongestUpslopeResultPath, TotalUpslopeResultPath, StrahlOrdResultPath, "", false, 8, false, null);

            if (ret == 0) return true;
            return false;
        }

        

        public bool RunDefineStreamGrids(int Threshold,
            string demPathWithMask, string demFillPath,
            string d8Path, string d8SlopePath, string areaD8Path,
            string areaDInf,
            string StrahlOrdResultPath,
            string LongestUpslopeResultPath,
            string TotalUpslopeResultPath,
            string StreamGridResultPath,
            string StreamOrdResultPath,
            string TreeDatResultPath,
            string CoordDatResultPath,
            string StreamShapeResultPath,
            string WatershedGridResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.DelinStreamGrids(demPathWithMask, demFillPath,
                d8Path, d8SlopePath, areaD8Path, areaDInf, "",
                StrahlOrdResultPath, LongestUpslopeResultPath, TotalUpslopeResultPath, StreamGridResultPath, StreamOrdResultPath,
                TreeDatResultPath, CoordDatResultPath, StreamShapeResultPath, WatershedGridResultPath, Threshold,
                false, false, true,
                8, false, null);

            if (ret == 0) return true;
            return false;
        }

        public bool RunAutoSnap(string outletPath, string newOutletPath, string d8Result, string streamShapeResult, double snapThreshod)
        {
            MapWinGIS.Grid g = new MapWinGIS.Grid();
            g.Open(d8Result);
            double dx = g.Header.dX;
            g.Close();
            return MapWinGeoProc.Utils.SnapPointsToLines(outletPath, streamShapeResult, snapThreshod, dx / 2, newOutletPath, true, null);
        }

        public bool RunAreaD8(string d8Path, string ad8ResultPath, string outletPath)
        {
            int ret = MapWinGeoProc.Hydrology.AreaD8(d8Path, outletPath, ad8ResultPath, true, false, 8, false, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunAreaDInf(string dInfPath, string adInfResultPath, string outletPath)
        {
            int ret = MapWinGeoProc.Hydrology.AreaDInf(dInfPath, outletPath, adInfResultPath, true, false, 8, false, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunDefineStreamGrids(int Threshold,
            string demPathWithMask, string demFillPath,
            string d8Path, string d8SlopePath, string areaD8Path, string areaDInf,
            string outletPath,
            string StrahlOrdResultPath,
            string LongestUpslopeResultPath,
            string TotalUpslopeResultPath,
            string StreamGridResultPath,
            string StreamOrdResultPath,
            string TreeDatResultPath,
            string CoordDatResultPath,
            string StreamShapeResultPath,
            string WatershedGridResultPath)
        {
            int ret = MapWinGeoProc.Hydrology.DelinStreamGrids(demPathWithMask, demFillPath,
                d8Path, d8SlopePath, areaD8Path, areaDInf,
                outletPath,
                StrahlOrdResultPath, LongestUpslopeResultPath, TotalUpslopeResultPath, StreamGridResultPath, StreamOrdResultPath,
                TreeDatResultPath, CoordDatResultPath, StreamShapeResultPath, WatershedGridResultPath, Threshold,
                true, false, true,
                8, false, null);

            if (ret == 0) return true;
            return false;
        }

        public bool RunWshedToShape(string d8Path, string watershedGrid, string waterShedShape)
        {
            int ret = MapWinGeoProc.Hydrology.SubbasinsToShape(d8Path, watershedGrid, waterShedShape, null);
            if (ret == 0) return true;
            return false;
        }

        public bool RunApplyStreamAttributes(string streamShape, string dem, string watershedShape)
        {
            return MapWinGeoProc.Hydrology.ApplyStreamAttributes(streamShape, dem, watershedShape, MapWinGeoProc.Hydrology.ElevationUnits.meters, null);
        }

        public bool RunApplyWatershedAttributes(string watershedShape, string streamShape)
        {
            int ret = MapWinGeoProc.Hydrology.ApplyWatershedLinkAttributes(watershedShape, streamShape, null);
            return false;
        }

        public bool RunBuildJoinedBasins(string watershedShape, string outletPath, string mergeWatershed)
        {
            return MapWinGeoProc.Hydrology.BuildJoinedBasins(watershedShape, outletPath, mergeWatershed, null);
        }

        public int GetNumCellsByDEMAndMask(MapWinGIS.GridHeader head, string strMask)
        {
            int numCells = 0, maskCells = 0;

            numCells = head.NumberCols * head.NumberRows;
            MapWinGIS.Shapefile sf = new MapWinGIS.Shapefile();
            sf.Open(strMask);
            for (int iShape = 0; iShape < sf.NumShapes; iShape++)
            {
                MapWinGIS.Shape tempShape = sf.Shape[iShape];
                int temp = Convert.ToInt32(MapWinGeoProc.Utils.Area(ref tempShape) / (head.dX * head.dY));
                maskCells = maskCells + temp;
            }
            sf.Close();
            if (numCells > maskCells)
                numCells = maskCells;

            return numCells;
        }

    }
}
