﻿//  Travel Time Analysis project
//  Copyright (C) 2010 Lukas Kabrt
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using LK.GeoUtils.Geometry;
using System;
using Xunit;

using LK.GeoUtils;

namespace GeoUtils.Tests {
		public class BBoxTest {
		[Fact()]
		public void BBoxExtendToCoverInflatesBBox() {
			PointGeo p1 = new PointGeo(12.1, 18.3, 100);
			PointGeo p2 = new PointGeo(-12.9, -34.3, 500);

			BBox target = new BBox();
			target.ExtendToCover(p1);
			target.ExtendToCover(p2);

			Assert.Equal(12.1, target.North);
			Assert.Equal(-12.9, target.South);
			Assert.Equal(18.3, target.East);
			Assert.Equal(-34.3, target.West);

			Assert.Equal(100, target.MinElevation);
			Assert.Equal(500, target.MaxElevation);
		}

		[Fact()]
		public void BBoxConstructorIEnumerableSetsBound() {
			PointGeo p1 = new PointGeo(15, 10, -100);
			PointGeo p2 = new PointGeo(-15, -10, 1000);

			BBox target = new BBox(new IPointGeo[] { p1, p2 });

			Assert.Equal(15, target.North);
			Assert.Equal(-15, target.South);
			Assert.Equal(10, target.East);
			Assert.Equal(-10, target.West);

			Assert.Equal(-100, target.MinElevation);
			Assert.Equal(1000, target.MaxElevation);
		}

		[Fact()]
		public void BBoxIsInsideReturnsTrueIfPointIsInside() {
			PointGeo p1 = new PointGeo(15, 10, -100);
			PointGeo p2 = new PointGeo(-15, -10, 1000);
			PointGeo pTest = new PointGeo(0, 0, 0);

			BBox target = new BBox(new IPointGeo[] { p1, p2 });

			Assert.True(target.IsInside(pTest));
		}

		[Fact()]
		public void BBoxIsInsideReturnsTrueForPointOnBoundary() {
			PointGeo p1 = new PointGeo(15, 10, -100);
			PointGeo p2 = new PointGeo(-15, -10, 1000);

			PointGeo pTestLat = new PointGeo(15, 0, 0);
			PointGeo pTestLat2 = new PointGeo(-15, 0, 0);
			PointGeo pTestLon = new PointGeo(0, 10, 0);
			PointGeo pTestLon2 = new PointGeo(0, -10, 0);
			PointGeo pTestEle = new PointGeo(0, 0, -100);
			PointGeo pTestEle2 = new PointGeo(0, 0, 1000);

			BBox target = new BBox(new IPointGeo[] { p1, p2 });

			Assert.True(target.IsInside(pTestLat), "Latitude");
			Assert.True(target.IsInside(pTestLon), "Longitude");
			Assert.True(target.IsInside(pTestEle), "Elevation");

			Assert.True(target.IsInside(pTestLat2), "Latitude");
			Assert.True(target.IsInside(pTestLon2), "Longitude");
			Assert.True(target.IsInside(pTestEle2), "Elevation");
		}

		[Fact()]
		public void BBoxIsInsideReturnsFalseIfPointIsOutside() {
			PointGeo p1 = new PointGeo(15, 10, -100);
			PointGeo p2 = new PointGeo(-15, -10, 1000);

			PointGeo pTestLat = new PointGeo(30, 0, 0);
			PointGeo pTestLon = new PointGeo(0, 30, 0);
			PointGeo pTestEle = new PointGeo(30, 0, 5000);


			BBox target = new BBox(new IPointGeo[] { p1, p2 });

			Assert.False(target.IsInside(pTestLat), "Latitude");
			Assert.False(target.IsInside(pTestLon), "Longitude");
			Assert.False(target.IsInside(pTestEle), "Elevation");
		}

		[Fact()]
		public void BBoxInflateIncreasesBBoxSize() {
			PointGeo p1 = new PointGeo(1, 0);
			PointGeo p2 = new PointGeo(0, 1);
			
			BBox target = new BBox(new IPointGeo[] { p1, p2 });

			target.Inflate(0.1, 0.2);
				
			Assert.Equal(1.1, target.North);
			Assert.Equal(-0.1, target.South);
			Assert.Equal(1.2, target.East);
			Assert.Equal(-0.2, target.West);
		}

		[Fact()]
		public void BBoxCornersReturnsCorners() {
			BBox target = new BBox() { North = 10, South = 8, East = 9, West = 7 };

			PointGeo corner1 = new PointGeo(10, 9);
			PointGeo corner2 = new PointGeo(10, 7);
			PointGeo corner3 = new PointGeo(8, 9);
			PointGeo corner4 = new PointGeo(8, 7);

			Assert.Contains(corner1, target.Corners);
			Assert.Contains(corner2, target.Corners);
			Assert.Contains(corner3, target.Corners);
			Assert.Contains(corner4, target.Corners);
		}


	}
}
