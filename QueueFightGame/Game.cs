using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFightGame
{
    //В этом классе можно реализовать Синглтон или лайзи, может быть только один Manager
    public class Game
    {
        private GameManager Manager;

        public Game()
        {
            Manager = new GameManager();
        }

        public void Initialize()
        {
            throw new NotImplementedException("Метод Initialize() не реализован");
        }

        public void Play()
        {
            throw new NotImplementedException("Метод Play() не реализован");
        }


    }
}
