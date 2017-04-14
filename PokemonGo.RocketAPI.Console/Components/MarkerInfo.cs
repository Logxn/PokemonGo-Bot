/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 09/02/2017
 * Time: 17:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using GMap.NET;

namespace PokeMaster.Components
{
    /// <summary>
    /// Description of MarkerInfo.
    /// </summary>
    public class MarkerInfo
    {
        public int type{get;set;}
        public string info{get;set;}
        public PointLatLng location{get;set;}
    }
}
