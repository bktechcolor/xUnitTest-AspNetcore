using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Test
{
    public static class WorldDumbestFunctionTests
    {
        //Naming Convention - ClassName_MethodName_ExpectResult
        public static void WorldDumbestFunction_ReturnsPikaChuIfZero_ReturnString()
        {
            try
            {
                //Arange - Go get your varibles, whatever you need, your classes, go get your functions
                int num = 0;
                WorldDumbestFunction worldDumbest = new WorldDumbestFunction();

                //Act - Execute this function
                string result = worldDumbest.ReturnsPikachuIfZero(num);

                //Assert - Whatever ever is returned is it what you want.
                if(result == "PIKACHU")
                {
                    Console.WriteLine("PASSED: WorldDumbestFunction.ReturnsPikaChuIfZero_ReturnString");
                }
                else
                {
                    Console.WriteLine("FAILED: WorldDumbestFunction.ReturnsPikaChuIfZero_ReturnString");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
