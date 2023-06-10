﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Config;

namespace ProspectorInfo.Models
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.None)]
    public class ProspectInfo
    {
        private static readonly Dictionary<RelativeDensity, string> RelativeDensityToLang = new Dictionary<RelativeDensity, string>{
                { RelativeDensity.VeryPoor, "propick-density-verypoor" },
                { RelativeDensity.Poor, "propick-density-poor"},
                { RelativeDensity.Decent, "propick-density-decent" },
                { RelativeDensity.High , "propick-density-high" },
                { RelativeDensity.VeryHigh , "propick-density-veryhigh" },
                { RelativeDensity.UltraHigh , "propick-density-ultrahigh" }
            };



        [ProtoBuf.ProtoMember(1)]
        public readonly ChunkCoordinate Chunk;

        /// <summary>
        /// A sorted list of all ore occurencies in this chunk. The ore with the highest relative density is first.
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public readonly List<OreOccurence> Values;

        /// <summary>
        /// The return value from <see cref="GetMessage"/> if it was called at least once. Used to avoid multiple StringBuilder calls.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        private string _MessageCache;

        /// <summary>
        /// Required for ProtoBuf deserialization
        /// </summary>
        private ProspectInfo()
        {
        }

        [Newtonsoft.Json.JsonConstructor]
        public ProspectInfo(ChunkCoordinate chunk, List<OreOccurence> values)
        {
            Chunk = chunk;
            Values = values;
        }

        public bool Equals(ProspectInfo other)
        {
            return Chunk.X == other.Chunk.X && Chunk.Z == other.Chunk.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is ProspectInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Chunk.X * 397 ^ Chunk.Z;
            }
        }

        public string GetMessage()
        {
            if (_MessageCache == null)
            {

                StringBuilder sb = new StringBuilder();

                if (Values.Count > 0)
                {
                    sb.AppendLine(Lang.Get("propick-reading-title", Values.Count));
                    List<string> traces = new List<string>();

                    foreach (var elem in Values)
                    {
                        if (elem.RelativeDensity > RelativeDensity.Miniscule)
                        {
                            sb.AppendLine(Lang.Get("propick-reading", Lang.Get(RelativeDensityToLang[elem.RelativeDensity]), elem.PageCode, Lang.Get(elem.Name), elem.AbsoluteDensity.ToString("0.#")));
                        }
                        else
                        {
                            traces.Add(Lang.Get(elem.Name));
                        }
                    }
                    if (traces.Count > 0)
                    {
                        sb.Append(Lang.Get("Miniscule amounts of {0}", string.Join(", ", traces)));
                        sb.AppendLine();
                    }
                }
                else
                {
                    sb.Append(Lang.Get("propick-noreading"));
                }

                _MessageCache = sb.ToString();
            }
            return _MessageCache;
        }

        public RelativeDensity GetValueOfOre(string oreName)
        {
            if (Values != null)
                foreach (var ore in Values)
                {
                    if (Lang.Get(ore.Name).ToLower() == oreName.ToLower() || ore.Name.ToLower() == oreName.ToLower())
                        return ore.RelativeDensity;
                }
            return RelativeDensity.Zero;
        }
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.None)]
    public readonly struct OreOccurence
    {
        [ProtoBuf.ProtoMember(1)]
        public readonly string Name;
        [ProtoBuf.ProtoMember(2)]
        public readonly string PageCode;
        [ProtoBuf.ProtoMember(3, IsRequired = true)]
        public readonly RelativeDensity RelativeDensity;
        [ProtoBuf.ProtoMember(4)]
        public readonly double AbsoluteDensity;

        [Newtonsoft.Json.JsonConstructor]
        public OreOccurence(string name, string pageCode, RelativeDensity relativeDensity, double absoluteDensity)
        {
            Name = name;
            PageCode = pageCode;
            RelativeDensity = relativeDensity;
            AbsoluteDensity = absoluteDensity;
        }
    }

    public enum RelativeDensity
    {
        Zero,
        Miniscule,
        VeryPoor,
        Poor,
        Decent,
        High,
        VeryHigh,
        UltraHigh
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.None)]
    public struct ChunkCoordinate
    {
        [ProtoBuf.ProtoMember(1)]
        public int X;
        [ProtoBuf.ProtoMember(2)]
        public int Z;

        public ChunkCoordinate(int x, int z)
        {
            X = x;
            Z = z;
        }
    }

    public class StoredData {

        public int Version = 1;

        public List<ProspectInfo> ProspectInfos = new List<ProspectInfo>();

        public StoredData() { 
        }

        [Newtonsoft.Json.JsonConstructor]
        public StoredData(int version, List<ProspectInfo> prospectInfos)
        {
            this.Version = version;
            this.ProspectInfos = prospectInfos;
        }

    }
}
