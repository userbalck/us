
package com.Seleium.www;
public class Assist {
	

	 public static void ty(int tiem) throws Exception {
		  try {
			  System.out.println("�ȴ��룺"+tiem);
			  int t=tiem*1000;
			   Thread.sleep(t);
			   
		} catch (Exception e) {
			// TODO: handle exception
		}
	 }
	 public static void pn(String s){
		 
		 System.out.println(s);
		 
	 }  
	 public static void pt(String s){
		 
		 System.out.print(s);
		 
	 }
	 
 }

