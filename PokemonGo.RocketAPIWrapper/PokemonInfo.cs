using System;
using System.Linq;
using POGOProtos.Data;

namespace PokemonGo.RocketAPIWrapper
{
    public static class PokemonInfo
    {

        public static double CalculatePokemonPerfection(PokemonData poke)
        {
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / 45.0 * 100.0;
        }

        public static int GetPowerUpLevel(PokemonData poke)
        {
            return (int)(GetLevel(poke) * 2.0);
        }
        public static double GetLevel(PokemonData poke)
        {

            switch ((int)((poke.CpMultiplier + poke.AdditionalCpMultiplier) * 1000.0)) {
                case 93: // 0.094 * 1000 = 93.99999678134
                case 94:
                    return 1;
                case 135:
                    return 1.5;
                case 166:
                    return 2;
                case 192:
                    return 2.5;
                case 215:
                    return 3;
                case 236:
                    return 3.5;
                case 255:
                    return 4;
                case 273:
                    return 4.5;
                case 290:
                    return 5;
                case 306:
                    return 5.5;
                case 321:
                    return 6;
                case 335:
                    return 6.5;
                case 349:
                    return 7;
                case 362:
                    return 7.5;
                case 375:
                    return 8;
                case 387:
                    return 8.5;
                case 399:
                    return 9;
                case 411:
                    return 9.5;
                case 422:
                    return 10;
                case 432:
                    return 15;
                case 443:
                    return 11;
                case 453:
                    return 11.5;
                case 462:
                    return 12;
                case 472:
                    return 12.5;
                case 481:
                    return 13;
                case 490:
                    return 13.5;
                case 499:
                    return 14;
                case 508:
                    return 14.5;
                case 517:
                    return 15;
                case 525:
                    return 15.5;
                case 534:
                    return 16;
                case 542:
                    return 16.5;
                case 550:
                    return 17;
                case 558:
                    return 17.5;
                case 566:
                    return 18;
                case 574:
                    return 18.5;
                case 582:
                    return 19;
                case 589:
                    return 19.5;
                case 597:
                    return 20;
                case 604:
                    return 25;
                case 612:
                    return 21;
                case 619:
                    return 21.5;
                case 626:
                    return 22;
                case 633:
                    return 22.5;
                case 640:
                    return 23;
                case 647:
                    return 23.5;
                case 654:
                    return 24;
                case 661:
                    return 24.5;
                case 667:
                    return 25;
                case 674:
                    return 25.5;
                case 681:
                    return 26;
                case 687:
                    return 26.5;
                case 694:
                    return 27;
                case 700:
                    return 27.5;
                case 706:
                    return 28;
                case 713:
                    return 28.5;
                case 719:
                    return 29;
                case 725:
                    return 29.5;
                case 731:
                    return 30;
                case 734:
                    return 35;
                case 737:
                    return 31;
                case 740:
                    return 31.5;
                case 743:
                    return 32;
                case 746:
                    return 32.5;
                case 749:
                    return 33;
                case 752:
                    return 33.5;
                case 755:
                    return 34;
                case 758:
                    return 34.5;
                case 761:
                    return 35;
                case 764:
                    return 35.5;
                case 767:
                    return 36;
                case 770:
                    return 36.5;
                case 773:
                    return 37;
                case 776:
                    return 37.5;
                case 778:
                    return 38;
                case 781:
                    return 38.5;
                case 784:
                    return 39;
                case 787:
                    return 39.5;
                case 790:
                    return 40;
                default:
                    return 0;
            }
        }
    }
}