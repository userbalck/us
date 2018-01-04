
package com.Seleium.www;
public class Assist {
	

	 public static void ty(int tiem) throws Exception {
		  try {
			  System.out.println("µÈ´ýÃë£º"+tiem);
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

