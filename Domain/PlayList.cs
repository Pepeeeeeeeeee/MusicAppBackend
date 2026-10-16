using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendMusic.Domain
{
    public class PlayList 
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<Songs> songs { get; set; }
        public PlayList() {
            songs = new List<Songs>();
        }
    }
}
