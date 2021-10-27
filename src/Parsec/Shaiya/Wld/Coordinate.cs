﻿using Parsec.Readers;
using Parsec.Shaiya.Common;

namespace Parsec.Shaiya.WLD
{
    /// <summary>
    /// Coordinates to place a 3D model in the world
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Id of a 3D Model
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// World position where to place the model
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Rotation vector
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Scaling vector - almost always (0, 1, 0)
        /// </summary>
        public Vector3 Scaling { get; set; }

        public Coordinate(ShaiyaBinaryReader binaryReader)
        {
            Id = binaryReader.Read<int>();
            Position = new Vector3(binaryReader);
            Rotation = new Vector3(binaryReader);
            Scaling = new Vector3(binaryReader);
        }
    }
}