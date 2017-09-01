using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test002快捷键使用
{
	class Program
	{
		static void Main(string[] args)
		{
			Jiujiu();
			Readwrite();
			//请求2个值交换，不适用第三方变量
			int n1 = 10;
			int n2 = 20;
			n1 = n1 - n2;
			Console.WriteLine("n1={0}", n1);
			n2 = n1 + n2;
			Console.WriteLine("n2={0}", n2);
			Console.WriteLine("-----------n1={0},n2={1}", n1, n2);
			n1 = n2 - n1;
			Console.WriteLine("n1={0}", n1);
			Console.WriteLine("n1={0},n2={1}", n1, n2);
			#region  z这些代码没用
			Console.WriteLine("123132");
			Console.WriteLine("123132"); Console.WriteLine("123132");
			Console.WriteLine("123132"); Console.WriteLine("ED123132"); Console.WriteLine("123132");
			Console.ReadKey();
			#endregion
		}
		public  static void Jiujiu() {
			for (int i=1;i<10;i++) {
				
				for (int j=1;j<i;j++) {
					
					Console.Write("{0}*{1}={2}\t", j,i,i*j);
				}
				Console.WriteLine();

			}

			Console.ReadKey();
		}

		public static void Readwrite(){
			int itRead=1;
			for (int i=1; i<100; i++) {
				Console.WriteLine("请输数字0-99");
				try
				{
					 itRead = Convert.ToInt32(Console.ReadLine());
				}
				catch {
					
					Console.WriteLine("请输入争取的制服");
				}
				
				Console.WriteLine("ni sh ru de shu zishi ={0}",itRead);
			}
			
		} 

		//接受控制面板输入
		public static void Reamd() {
			Console.WriteLine("输入你的名字");
			string st=Console.ReadLine();
			Console.WriteLine("你的名字{0}",st);
			Console.ReadKey();

		}
		//练习题计算几天几周
		public static void Jvd()
		{
			
			string st = "输入你的名字第三方";
			Console.WriteLine("你的名字{0}", st);
			Console.ReadKey();

		}

	}

} 
