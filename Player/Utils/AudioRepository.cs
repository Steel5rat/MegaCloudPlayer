using System;
using System.Collections.Generic;
using System.IO;
using CG.Web.MegaApiClient;
using Player.Models;

namespace Player.Utils
{
    public class AudioRepository : IDisposable
    {
        private readonly Node _model;
        private readonly List<INode> _nodes;

        private byte[] nextCached;
        private bool nextInitialized = false;

        private byte[] currentCached;
        private byte[] previousCached;
        private int playlistPointer = -1;

        public AudioRepository(List<INode> nodes, Node model)
        {
            _nodes = nodes;
            _model = model;
        }

        public byte[] Next()
        {
            lock ((object)nextInitialized)
            {
                playlistPointer++;
                if (playlistPointer >= _nodes.Count)
                {
                    return null;
                }
                ;
                if (!nextInitialized)
                {
                    using (var stream = _model.GetNodeStream(_nodes[playlistPointer]))
                    {
                        using (var br = new BinaryReader(stream))
                        {
                            nextCached = br.ReadBytes((int)stream.Length);
                        }
                    }
                }
                previousCached = currentCached;
                currentCached = nextCached;


                return currentCached;
            }
        }

        public byte[] Previous()
        {
        }

    }
}