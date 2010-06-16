﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LK.GeoUtils;
using LK.GeoUtils.Geometry;
using LK.GPXUtils;
using LK.OSMUtils.OSMDatabase;

namespace LK.MatchGPX2OSM {
	public class STMatching {
		public static int MaxCandidatesCount = 5;

		RoadGraph _graph;
		CandidatesGraph _candidatesGraph;

		public STMatching(RoadGraph graph) {
			_graph = graph;
		}

		public OSMDB Reconstruct(IEnumerable<CandidatePoint> matched) {
			OSMDB result = new OSMDB();
			int counter = -1;



			return result;
		}
		
		/// <summary>
		/// Matches the given GPX track to the map
		/// </summary>
		/// <param name="gpx"></param>
		/// <returns>OSM db that represents matched track</returns>
		public OSMDB Match(GPXTrackSegment gpx) {
			IList<CandidatePoint> matched = FindMatchedCandidates(gpx).ToList();
			
			OSMDB result = new OSMDB();
			int counter = -1;

			Astar pathfinder = new Astar(_graph);

			OSMNode node = new OSMNode(counter--, matched[0].Latitude, matched[0].Longitude);
			node.Tags.Add(new OSMTag("time", matched[0].Layer.TrackPoint.Time.ToString()));
			result.Nodes.Add(node);

			for (int i = 0; i < matched.Count - 1; i++) {
				if (counter < -90) {
					int a = 0;
				}

				OSMWay track = new OSMWay(counter--);
				result.Ways.Add(track);
				track.Nodes.Add(node.ID);

				if (Calculations.GetDistance2D(matched[i + 1], matched[i].Road) < 0.01) {
					var points = GetNodesBetweenPoints(matched[i], matched[i + 1], matched[i].Road).ToList();
					foreach (var point in points) {
						node = new OSMNode(counter--, point.Latitude, point.Longitude);
						result.Nodes.Add(node);
						track.Nodes.Add(node.ID);
					}
				}
				else if (Calculations.GetDistance2D(matched[i], matched[i+1].Road) < 0.01) {
					var points = GetNodesBetweenPoints(matched[i], matched[i + 1], matched[i+1].Road).ToList();
					foreach (var point in points) {
						node = new OSMNode(counter--, point.Latitude, point.Longitude);
						result.Nodes.Add(node);
						track.Nodes.Add(node.ID);
					}
				}
				else {
					if (counter < -90) {
						int a = 0;
					}
					double lenght = 0;
					var paths = pathfinder.FindPath(matched[i], matched[i + 1], ref lenght).ToList();

					for(int j = 0; j < paths.Count; j++) {
						if (j > 0) {
							node = new OSMNode(counter--, paths[j].From.Position.Latitude, paths[j].From.Position.Longitude);
							result.Nodes.Add(node);
							track.Nodes.Add(node.ID);
						}
						
						var points = GetNodesBetweenPoints(paths[j].From.Position, paths[j].To.Position, paths[j].Road).ToList();
						foreach (var point in points) {
							node = new OSMNode(counter--, point.Latitude, point.Longitude);
							result.Nodes.Add(node);
							track.Nodes.Add(node.ID);
						}
					}
				}

				node = new OSMNode(counter--, matched[i + 1].Latitude, matched[i + 1].Longitude);
				node.Tags.Add(new OSMTag("time", matched[i + 1].Layer.TrackPoint.Time.ToString()));
				result.Nodes.Add(node);
				track.Nodes.Add(node.ID);
			}

			return result;
		}

		IEnumerable<IPointGeo> GetNodesBetweenPoints(IPointGeo from, IPointGeo to, IPolyline<IPointGeo> path) {
			var segments = path.Segments;

			List<IPointGeo> result = new List<IPointGeo>();

			int fromIndex = -1;
			int toIndex = -1;
			for (int i = 0; i < segments.Count; i++) {
				if (Calculations.GetDistance2D(from, segments[i]) < Calculations.EpsLength) {
					if (fromIndex > -1 && toIndex > -1 && toIndex < fromIndex)
						;
					else
						fromIndex = i;
				}
				if (Calculations.GetDistance2D(to, segments[i]) < Calculations.EpsLength) {
					if (fromIndex > -1 && toIndex > -1 && toIndex > fromIndex)
						;
					else
						toIndex = i;
				}
			}
			if (fromIndex == -1 || toIndex == -1)
				return result;

			if (fromIndex == toIndex - 1) {
				result.Add(segments[fromIndex].EndPoint);
			}
			else if (fromIndex - 1 == toIndex) {
				result.Add(segments[toIndex].EndPoint);
			}
			else if (fromIndex < toIndex) {
				for (int i = fromIndex; i < toIndex; i++) {
					result.Add(segments[i].EndPoint);
				}
			}
			else if (toIndex < fromIndex) {
				for (int i = fromIndex; i > toIndex; i--) {
					result.Add(segments[i].StartPoint);
				}
			}

			return result;
		}
		/// <summary>
		/// Finds the best matching sequence of candidate points
		/// </summary>
		/// <param name="gpx">The GPS track</param>
		/// <returns></returns>
		IEnumerable<CandidatePoint> FindMatchedCandidates(GPXTrackSegment gpx) {
			_candidatesGraph = new CandidatesGraph();

			//Find candidate points + ObservationProbability
			foreach (var gpxPoint in gpx.Nodes) {
				var candidates = FindCandidatePoints(gpxPoint);

				_candidatesGraph.Layers.Add(CreateLayer(gpxPoint, candidates));
			}

			_candidatesGraph.ConnectLayers();
			AssignTransmissionProbability();

			// Find matched sequence
			foreach (var candidate in _candidatesGraph.Layers[0].Candidates) {
				candidate.HighestProbability = candidate.ObservationProbability;
			}
			for (int i = 0; i < _candidatesGraph.Layers.Count - 1; i++) {
				foreach (var candidate in _candidatesGraph.Layers[i + 1].Candidates) {
					foreach (var connection in candidate.IncomingConnections) {
						double score = connection.From.HighestProbability + candidate.ObservationProbability * connection.TransmissionProbability;
						if (score > candidate.HighestProbability) {
							candidate.HighestProbability = score;
							candidate.HighesScoreParent = connection.From;
						}
					}
				}
			}

			List<CandidatePoint> result = new List<CandidatePoint>();
			CandidatePoint current = new CandidatePoint() { HighestProbability = double.NegativeInfinity };
			foreach (var point in _candidatesGraph.Layers[_candidatesGraph.Layers.Count - 1].Candidates) {
				if (point.HighestProbability > current.HighestProbability) {
					current = point;
				}
			}

			while (current != null) {
				result.Add(current);
				current = current.HighesScoreParent;
			}

			result.Reverse();
			return result;
		}

		/// <summary>
		/// Finds all candidates points for given GPS track point
		/// </summary>
		/// <param name="gpxPt">GPS point</param>
		/// <returns>Collection of points candidate points on road segments</returns>
		public IEnumerable<CandidatePoint> FindCandidatePoints(GPXPoint gpxPt) {
			List<CandidatePoint> result = new List<CandidatePoint>();
			BBox gpxBbox = new BBox(new IPointGeo[] { gpxPt });
			gpxBbox.Inflate(0.0007, 0.0011);

			foreach (var road in _graph.ConnectionGeometries) {
				if (Topology.Intersects(gpxBbox, road.BBox)) {
					IPointGeo projectedPoint = Topology.ProjectPoint(gpxPt, road);
					result.Add(new CandidatePoint() { Latitude = projectedPoint.Latitude, Longitude = projectedPoint.Longitude, Road = road, ObservationProbability = CalculateObservationProbability(gpxPt, projectedPoint) });
				}
			}

			return result;
		}

		/// <summary>
		/// Creates a new layer
		/// </summary>
		/// <param name="originalPoint">GPX track point</param>
		/// <param name="candidates">Candidate points for the original point</param>
		/// <returns></returns>
		CandidateGraphLayer CreateLayer(GPXPoint originalPoint, IEnumerable<CandidatePoint> candidates) {
			CandidateGraphLayer result = new CandidateGraphLayer() { TrackPoint = originalPoint };
			result.Candidates.AddRange(candidates.OrderByDescending(c => c.ObservationProbability).Take(Math.Min(candidates.Count(), STMatching.MaxCandidatesCount)));

			foreach (var candidate in result.Candidates) {
				candidate.Layer = result;
			}

			return result;
		}

		/// <summary>
		/// Calculates observation probability
		/// </summary>
		/// <param name="original">GPS track point</param>
		/// <param name="candidate">Candidate point</param>
		/// <returns>double representing probability that GPS track point corresponds with Candidate point</returns>
		double CalculateObservationProbability(GPXPoint original, IPointGeo candidate) {
			double sigma = 30;
			double distance = Calculations.GetDistance2D(original, candidate);
			return Math.Exp(-distance * distance / (2 * sigma * sigma)) / (sigma * Math.Sqrt(Math.PI * 2));
		}

		/// <summary>
		/// Assigns transmission probability to every connection in the graph
		/// </summary>
		void AssignTransmissionProbability() {
			foreach (var layer in _candidatesGraph.Layers) {
				foreach (var candidatePoint in layer.Candidates) {
					foreach (var connection in candidatePoint.OutgoingConnections) {
						connection.TransmissionProbability = CalculateTransmissionProbability(connection);
					}
				}
			}
		}
		/// <summary>
		/// Calculates transmission probability for connection
		/// </summary>
		/// <param name="c">Connection</param>
		/// <returns>double value representing transmission probability</returns>
		double CalculateTransmissionProbability(CandidatesConection c) {
			double gcd = Calculations.GetDistance2D(c.From, c.To);
			double shortestPath = FindShortestPath(c.From, c.To);

			if (gcd == 0 && shortestPath == 0)
				return 1;
			else
				return gcd / shortestPath;
		}

		/// <summary>
		/// Finds shortest path between two points along routes
		/// </summary>
		/// <param name="from">Start point</param>
		/// <param name="to">Destination point</param>
		/// <returns>length of the path in meters</returns>
		double FindShortestPath(CandidatePoint from, CandidatePoint to) {
			if (from.Road == to.Road) {
				return Calculations.GetPathLength(from, to, from.Road);
			}
			else {
				Astar pathfinder = new Astar(_graph);
				double length = double.PositiveInfinity;
				pathfinder.FindPath(from, to, ref length);
				return length;
			}
		}
	}
}
