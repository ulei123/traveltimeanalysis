﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using LK.GeoUtils;
using LK.GeoUtils.Geometry;

namespace GeoUtils.Tests {
	public class TopologyTest {
		[Fact()]
		public void TopologyProjectPointReturnsSegmentOrginForPointsOutsideSegment() {
			PointGeo orgin = new PointGeo(5, 10);
			PointGeo end = new PointGeo(5, 15);
			Segment<IPointGeo> segment = new Segment<IPointGeo>(orgin, end);

			PointGeo testPoint1 = new PointGeo(5, 8);
			PointGeo testPoint2 = new PointGeo(6, 10);
			PointGeo testPoint3 = new PointGeo(4, 10);

			Assert.Equal(orgin, Topology.ProjectPoint(testPoint1, segment));
			Assert.Equal(orgin, Topology.ProjectPoint(testPoint2, segment));
			Assert.Equal(orgin, Topology.ProjectPoint(testPoint3, segment));
		}

		[Fact()]
		public void TopologyProjectPointReturnsSegmentOrginForPointsOutsideSegment2() {
			PointGeo orgin = new PointGeo(10, 5);
			PointGeo end = new PointGeo(15, 5);
			Segment<IPointGeo> segment = new Segment<IPointGeo>(orgin, end);

			PointGeo testPoint1 = new PointGeo(8, 5);
			PointGeo testPoint2 = new PointGeo(10, 6);
			PointGeo testPoint3 = new PointGeo(10, 4);

			Assert.Equal(orgin, Topology.ProjectPoint(testPoint1, segment));
			Assert.Equal(orgin, Topology.ProjectPoint(testPoint2, segment));
			Assert.Equal(orgin, Topology.ProjectPoint(testPoint3, segment));
		}

		[Fact()]
		public void TopologyProjectPointReturnsSegmentEndForPointsOutsideSegment() {
			PointGeo orgin = new PointGeo(5, 10);
			PointGeo end = new PointGeo(5, 15);
			Segment<IPointGeo> segment = new Segment<IPointGeo>(orgin, end);

			PointGeo testPoint1 = new PointGeo(5, 17);
			PointGeo testPoint2 = new PointGeo(6, 15);
			PointGeo testPoint3 = new PointGeo(4, 15);

			Assert.Equal(end, Topology.ProjectPoint(testPoint1, segment));
			Assert.Equal(end, Topology.ProjectPoint(testPoint2, segment));
			Assert.Equal(end, Topology.ProjectPoint(testPoint3, segment));
		}


		[Fact()]
		public void TopologyProjectPointReturnsSegmentEndForPointsOutsideSegment2() {
			PointGeo orgin = new PointGeo(10, 5);
			PointGeo end = new PointGeo(15, 5);
			Segment<IPointGeo> segment = new Segment<IPointGeo>(orgin, end);

			PointGeo testPoint1 = new PointGeo(17, 5);
			PointGeo testPoint2 = new PointGeo(15, 6);
			PointGeo testPoint3 = new PointGeo(15, 4);

			Assert.Equal(end, Topology.ProjectPoint(testPoint1, segment));
			Assert.Equal(end, Topology.ProjectPoint(testPoint2, segment));
			Assert.Equal(end, Topology.ProjectPoint(testPoint3, segment));
		}

		[Fact()]
		public void TopologyProjectPointReturnsCorrrectPointOnSegment() {
			PointGeo orgin = new PointGeo(1, 1);
			PointGeo end = new PointGeo(2, 2);
			Segment<IPointGeo> segment = new Segment<IPointGeo>(orgin, end);

			PointGeo testPoint1 = new PointGeo(1, 2);
			PointGeo testPointExpected1 = new PointGeo(1.5,1.5);
			PointGeo testPoint2 = new PointGeo(2, 1);
			PointGeo testPointExpected2 = new PointGeo(1.5,1.5);
			
			Assert.Equal(testPointExpected1, Topology.ProjectPoint(testPoint1, segment));
			Assert.Equal(testPointExpected2, Topology.ProjectPoint(testPoint2, segment));
		}
	}
}