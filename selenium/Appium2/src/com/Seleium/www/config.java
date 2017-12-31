package com.Seleium.www;

public class config {
	static String Browser;
	public static String userad,htp,pwd,asp;
	public static int waitTime;
	static {

		Parsexml px = new Parsexml("config/config.xml");
		htp=px.getElementtext("/*/http");
		Browser = px.getElementtext("/*/browser");
		// 获取延时的是String数据，强转int
		waitTime = Integer.valueOf(px.getElementtext("/*/waitTime"));
		userad = px.getElementtext("/*/adminuser");
		pwd=px.getElementtext("/*/passpwd");
		asp=px.getElementtext("/*/asp");
		
	}
}
