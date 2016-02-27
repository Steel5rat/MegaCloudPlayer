using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using CG.Web.MegaApiClient;

namespace Player.Models
{
    public class Node
    {
        private readonly MegaApiClient _client = new MegaApiClient();
        private readonly IEnumerable<INode> _nodes;

        public Node()
        {
            _client.Login(ConfigurationManager.AppSettings["CloudLogin"],
                ConfigurationManager.AppSettings["CloudPassword"]);
            _nodes = _client.GetNodes();
        }

        public IEnumerable<INode> GetRootDirs()
        {
            var root = _nodes.Single(n => n.Type == NodeType.Root);
            return _nodes.Where(w => w.ParentId == root.Id && w.Type == NodeType.Directory);
        }

        public Stream GetMp3()
        {
            var node = _nodes.First(w => w.Type == NodeType.File && w.Name.Split('.').Last().ToUpper() == "MP3");
            return _client.Download(node);
        }

        public Stream GetNodeStream(INode node)
        {
            return _client.Download(node);
        }

        public IList<INode> GetAllMp3(IList<string> dirIds)
        {
            return GetNodesFromDirs(dirIds, "MP3");
        }

        private IList<INode> GetNodesFromDirs(IEnumerable<string> dirIds, string extension)
        {
            List<INode> result = null;
            foreach (var dirId in dirIds)
            {
                result = _nodes.Where(
                    w => w.ParentId == dirId && w.Type == NodeType.File && w.Name.Split('.').Last().ToUpper() == extension)
                    .ToList();
                result.AddRange(GetNodesFromDirs(_nodes.Where(w => w.ParentId == dirId && w.Type == NodeType.Directory).Select(s => s.Id), extension));
            }
            return result;
        }
    }
}