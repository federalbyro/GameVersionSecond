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
            Fight();
        }

        public void CreatFighters()
        {
            WeakFighter weakFighter1 = new WeakFighter();
            WeakFighter weakFighter2 = new WeakFighter();
            WeakFighter weakFighter3 = new WeakFighter();
            WeakFighter weakFighter4 = new WeakFighter();

            redTeam.AddFighter(weakFighter1);
            redTeam.AddFighter(weakFighter2);

            blueTeam.AddFighter(weakFighter3);
            blueTeam.AddFighter(weakFighter4);
        }

        public void Fight()
        {
            while (redTeam.HasFighters() && blueTeam.HasFighters())
            {
                IUnit redFighter = redTeam.GetCurrentFighter();
                IUnit blueFighter = blueTeam.GetCurrentFighter();

                Console.WriteLine($"\n{redFighter.Name} (Health: {redFighter.Health}) vs {blueFighter.Name} (Health: {blueFighter.Health})");

                redFighter.Attack(blueFighter);
                if (blueFighter.Health <= 0)
                {
                    Console.WriteLine($"{blueFighter.Name} погиб!");
                    blueTeam.RemoveFighter();
                }

                if (!blueTeam.HasFighters()) break;

                blueFighter = blueTeam.GetCurrentFighter();
                blueFighter.Attack(redFighter);
                if (redFighter.Health <= 0)
                {
                    Console.WriteLine($"{redFighter.Name} погиб!");
                    redTeam.RemoveFighter();
                }
            }
        }


    }
}
