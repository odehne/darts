using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darts
{
	public class Checkouts : List<Checkout>
	{
		
		public string GetDartOut(int value)
		{
			var result = this.FirstOrDefault(v => v.Score.Equals(value));
			if (result != null)
			{
				return result.OutOption;
			}
			return "";	
		}

		public Checkouts()
		{

			Add(new Checkout(170, "T20, T20, Bullseye"));
			Add(new Checkout(167,"T20, T19, Bullseye"));
			Add(new Checkout(164,"T20, T18, Bullseye"));
			Add(new Checkout(161, "T20, T17, Bullseye"));
			Add(new Checkout(160,"T20, T20, D20"));
			Add(new Checkout(158,"T20, T20, D19"));
			Add(new Checkout(157,"T20, T19, D20"));
			Add(new Checkout(156,"T20, T20, D18"));
			Add(new Checkout(155,"T20, T19, D19"));
			Add(new Checkout(154,"T20, T18, D20"));
			Add(new Checkout(153,"T20, T19, D18"));
			Add(new Checkout(152,"T20, T20, D16"));
			Add(new Checkout(151,"T20, T17, D20"));
			Add(new Checkout(150,"T20, T18, D18"));
			Add(new Checkout(149,"T20, T19, D16"));
			Add(new Checkout(148,"T20, T20, D14"));
			Add(new Checkout(147,"T20, T17, D18"));
			Add(new Checkout(146,"T20, T18, D16"));
			Add(new Checkout(145,"T20, T19, D14"));
			Add(new Checkout(144,"T20, T20, D12"));
			Add(new Checkout(143,"T19, T18, D16"));
			Add(new Checkout(142,"T20, T14, D20"));
			Add(new Checkout(141,"T20, T19, D12"));
			Add(new Checkout(140,"T20, T20, D10"));
			Add(new Checkout(139,"T19, T14, D20"));
			Add(new Checkout(138,"T20, T18, D12"));
			Add(new Checkout(137, "T20, T19, D10"));
			Add(new Checkout(136, "T20, T20, D8"));
			Add(new Checkout(135, "T20, T17, D12"));
			Add(new Checkout(134, "T20, T14, D16"));
			Add(new Checkout(133, "T20, T19, D8"));
			Add(new Checkout(132, "T20, T16, D12"));
			Add(new Checkout(131,"T20, T13, D16"));
			Add(new Checkout(130,"T20, T18, D8"));
			Add(new Checkout(129,"T19, T16, D12"));
			Add(new Checkout(128,"T18, T14, D16"));
			Add(new Checkout(127,"T20, T17, D8"));
			Add(new Checkout(126,"T19, T15, D12"));
			Add(new Checkout(125,"T18, T13, D16"));
			Add(new Checkout(124,"T20, T16, D8"));
			Add(new Checkout(123,"T19, T14, D12"));
			Add(new Checkout(122,"T18, T18, D7"));
			Add(new Checkout(121,"T20, T11, D14"));
			Add(new Checkout(120,"T20, S20, D20"));
			Add(new Checkout(119,"T19, T12, D13"));
			Add(new Checkout(118,"T20, S18, D20"));
			Add(new Checkout(117,"T20, S17,D20"));
			Add(new Checkout(116,"T20, S16, D20"));
			Add(new Checkout(115,"T19, S18, D20"));
			Add(new Checkout(114,"T20, S14, D20"));
			Add(new Checkout(113,"T19, S16, D20"));
			Add(new Checkout(112,"T20, T12, D8"));
			Add(new Checkout(111,"T19, S14, D20"));
			Add(new Checkout(110,"T20, S10, D20"));
			Add(new Checkout(109,"T20, S9, D20"));
			Add(new Checkout(108,"T19, S19, D16"));
			Add(new Checkout(107,"T20, S15, D16"));
			Add(new Checkout(106,"T20, S6, D20"));
			Add(new Checkout(105,"T19, S8, D20"));
			Add(new Checkout(104,"T20, T12, D4"));
			Add(new Checkout(103,"T17, S12, D20"));
			Add(new Checkout(102,"T20, S10, D16"));
			Add(new Checkout(101,"T17, S10, D20"));
			Add(new Checkout(100,"T20, D20"));
			Add(new Checkout(99 , "T19, S10, D16"));
			Add(new Checkout(98 , "T20, D19"));
			Add(new Checkout(97 ,"T19, D20"));
			Add(new Checkout(96 ,"T20, D18"));
			Add(new Checkout(95 ,"T19, D19"));
			Add(new Checkout(94 ,"T18, D20"));
			Add(new Checkout(93 ,"T19, D18"));
			Add(new Checkout(92 ,"T20, D16"));
			Add(new Checkout(91 ,"T17, D20"));
			Add(new Checkout(91 ,"T17, D20"));
			Add(new Checkout(89 ,"T19, D16"));
			Add(new Checkout(88 ,"T16, D20"));
			Add(new Checkout(87 ,"T17, D18"));
			Add(new Checkout(86 ,"T18, D16"));
			Add(new Checkout(85 ,"T15, D20"));
			Add(new Checkout(84 ,"T20, D12"));
			Add(new Checkout(83 ,"T17, D16"));
			Add(new Checkout(82 ,"T14, D20"));
			Add(new Checkout(81 ,"T19, D12"));
			Add(new Checkout(80 ,"T20, D10"));
			Add(new Checkout(79 ,"T13, D20"));
			Add(new Checkout(78 ,"T18, D12"));
			Add(new Checkout(77 , "T15, D16"));
			Add(new Checkout(76, "T20, D8"));
			Add(new Checkout(75 ,"T17, D12"));
			Add(new Checkout(74 , "T14, D16"));
			Add(new Checkout(73 , "T19, D8"));
			Add(new Checkout(72 , "T16, D12"));
			Add(new Checkout(71 , "T13, D16"));
			Add(new Checkout(70 ,"T18, D8"));
			Add(new Checkout(69 ,"T19, D6"));
			Add(new Checkout(68 ,"T20, D4"));
			Add(new Checkout(67 , "T17, D8"));
			Add(new Checkout(66 , "T14, D12"));
			Add(new Checkout(65 , "T19, D4"));
			Add(new Checkout(64 , "T16, D8"));
			Add(new Checkout(63 , "T13, D12"));
			Add(new Checkout(62 , "T10, D16"));
			Add(new Checkout(61 , "T15, D8"));
			Add(new Checkout(60 , "S20, D20"));
			Add(new Checkout(59 , "S19, D20"));
			Add(new Checkout(58 ,"S18, D20"));
			Add(new Checkout(57, "S17, D20"));
			Add(new Checkout(56, "T16, D4"));
			Add(new Checkout(55 ,"S15, D20"));
			Add(new Checkout(54 ,"S14, D20"));
			Add(new Checkout(53 ,"S12, D20"));
			Add(new Checkout(52 ,"S12, D20"));
			Add(new Checkout(51 ,"S11, D20"));
			Add(new Checkout(50 , "S10, D20"));
			Add(new Checkout(49, "S9, D20"));
			Add(new Checkout(48, "S8, D20"));
			Add(new Checkout(47 ,"S15, D16"));
			Add(new Checkout(46 ,"S14, D16"));
			Add(new Checkout(45 ,"S13, D16"));
			Add(new Checkout(44 ,"S12, D16"));
			Add(new Checkout(43 ,"S11, D16"));
			Add(new Checkout(42 , "S10, D16"));
			Add(new Checkout(41, "S9, D16"));
			Add(new Checkout(39 ,"S7, D16"));
			Add(new Checkout(37 ,"S5, D16"));
			Add(new Checkout(35 ,"S3, D16"));
			Add(new Checkout(33 ,"S1, D16"));
			Add(new Checkout(31 ,"S7, D12"));
			Add(new Checkout(29 ,"S13, D8"));
			Add(new Checkout(27 , "S11, D8"));
			Add(new Checkout(25, "S9, D8"));
			Add(new Checkout(23 ,"S7, D8"));
			Add(new Checkout(21 ,"S5 ,D8"));
			Add(new Checkout(19 , "S3, D8"));
			Add(new Checkout(17, "S13, D2"));
			Add(new Checkout(15 ,"S7, D4"));
			Add(new Checkout(13 ,"S5, D4"));
			Add(new Checkout(11 ,"S3, D4"));
			Add(new Checkout(9  ,"S1, D4"));
			Add(new Checkout(7  ,"S3, D2"));
			Add(new Checkout(5  ,"S1, D2"));
			Add(new Checkout(3  , "S1, D1"));
			
			for (int i = 2; i < 40; i=i+2)
			{
				Add(new Checkout(i, $"D{i / 2}"));
			}
		}

		//public string GetDouble(int value)
		//{
		//	if (value == 50)
		//		return "BULL";
		//	if (value > 20 || value < 2)
		//		return "";
		//	if (value % 2 != 0)
		//		return "";
		//	return $"D{value / 2}";
		//}

		//public string GetTripple(int value)
		//{
		//	if (value > 20 || value < 2)
		//		return "";
		//	if (value % 2 != 0)
		//		return "";
		//	return $"T{value / 2}";
		//}

	}
	public class Checkout
	{
		public int Score { get; set; }
		public string OutOption { get; set; }
	
		public Checkout(int score, string option)
		{
			Score = score;
			OutOption = option;
		}

		public Checkout()
		{

		}
	}
}
