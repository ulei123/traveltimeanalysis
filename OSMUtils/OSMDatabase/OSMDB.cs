﻿using System;
using System.Collections.Generic;
using System.Text;

using OSMUtils.OSMDataSource;
using System.IO;

namespace OSMUtils.OSMDatabase {
	/// <summary>
	/// Represents an OSM database
	/// </summary>
	public class OSMDB {
		/// <summary>
		/// Creates a new OSMDB object
		/// </summary>
		public OSMDB() {
			_nodes = new OSMObjectCollection<OSMNode>();
			_ways = new OSMObjectCollection<OSMWay>();
			_relations = new OSMObjectCollection<OSMRelation>();
		}

		/// <summary>
		/// Loads OSM entities from the specific OSM file
		/// </summary>
		/// <param name="filename">Path to the OSM file</param>
		public void Load(string filename) {
			OSMXmlDataReader xmlReader = new OSMXmlDataReader();
			xmlReader.NodeRead += new OSMNodeReadHandler(node => _nodes.Add(node));
			xmlReader.WayRead += new OSMWayReadHandler(way => _ways.Add(way));
			xmlReader.RelationRead += new OSMRelationReadHandler(relation => _relations.Add(relation));

			xmlReader.Read(filename);
		}

        /// <summary>
        /// Loads OSM entities from the specific OSM file
        /// </summary>
        /// <param name="filename">Path to the OSM file</param>
        public void Load(Stream stream) {
            OSMXmlDataReader xmlReader = new OSMXmlDataReader();
            xmlReader.NodeRead += new OSMNodeReadHandler(node => _nodes.Add(node));
            xmlReader.WayRead += new OSMWayReadHandler(way => _ways.Add(way));
            xmlReader.RelationRead += new OSMRelationReadHandler(relation => _relations.Add(relation));

            xmlReader.Read(new StreamReader(stream));
        }

		/// <summary>
		/// Saves OSM Database to the specific OSM file
		/// </summary>
		/// <param name="filename">Path to the OSM file</param>
		public void Save(string filename) {
			using (OSMXmlDataWriter writer = new OSMXmlDataWriter(filename)) {

				foreach (var node in _nodes) {
					writer.WriteNode(node);
				}

				foreach (var way in _ways) {
					writer.WriteWay(way);
				}

				foreach (var relation in _relations) {
					writer.WriteRelation(relation);
				}

				writer.Close();
			}
		}

        /// <summary>
        /// Saves OSM Database to the specific OSM file
        /// </summary>
        /// <param name="filename">Path to the OSM file</param>
        public void Save(Stream stream) {
            using (OSMXmlDataWriter writer = new OSMXmlDataWriter(stream)) {

                foreach (var node in _nodes) {
                    writer.WriteNode(node);
                }

                foreach (var way in _ways) {
                    writer.WriteWay(way);
                }

                foreach (var relation in _relations) {
                    writer.WriteRelation(relation);
                }

                writer.Close();
            }
        }

		protected OSMObjectCollection<OSMNode> _nodes;
		/// <summary>
		/// Gets the collection of OSMNodes
		/// </summary>
		public OSMObjectCollection<OSMNode> Nodes {
			get {
				return _nodes;
			}
		}

		OSMObjectCollection<OSMWay> _ways;
		/// <summary>
		/// Gets the collection of OSMWays
		/// </summary>
		public OSMObjectCollection<OSMWay> Ways {
			get {
				return _ways;
			}
		}


		OSMObjectCollection<OSMRelation> _relations;
		/// <summary>
		/// Get the collection of OSMRelations
		/// </summary>
		public OSMObjectCollection<OSMRelation> Relations {
			get {
				return _relations;
			}
		}

	}
}
