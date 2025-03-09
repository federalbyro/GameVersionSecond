using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    internal class GameManager
    {

        private Team redTeam;
        private Team blueTeam;

        public GameManager()
        {
            redTeam = new Team("Red");
            blueTeam = new Team("Blue");

            CreatFighters();
        }

        public void CreatFighters()
        {
            WeakFighter weakFighter1 = new WeakFighter();
            WeakFighter weakFighter2 = new WeakFighter();

            redTeam.AddFighter(weakFighter1);
            blueTeam.AddFighter(weakFighter2);
        }

        public void Fight()
        {

        }
    }
}
