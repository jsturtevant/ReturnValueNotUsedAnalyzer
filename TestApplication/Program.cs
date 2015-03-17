using System;

namespace TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var enterprise = SpaceShipFactory.CreateSpaceship(ShipTypes.USSEnterprise);
            Console.WriteLine(enterprise.ShipName);

            SpaceShipFactory.CreateSpaceship(ShipTypes.USSndromeda);

            Console.ReadLine();
        }
    }

    public class SpaceShipFactory
    {
        public static Ship CreateSpaceship(ShipTypes type)
        {
            return new Ship(type.ToString());
        }
    }

    public class Ship
    {
        public Ship(string name)
        {
            this.ShipName = name;
        }

        public string ShipName { get; } 
    }

    public enum ShipTypes
    {
        USSEnterprise,
        USSndromeda
    }
}
