package com.Seleium.www;

public class Config {
	static String Browser,app;
	public static String userad,htp,pwd,asp,Package,Activity;
	public static int waitTime;
	public Config() {
		Assist.pn("Config");
		Parsexml px = new Parsexml("config/config.xml");
		htp=px.getElementtext("/*/http");
		Browser = px.getElementtext("/*/browser");
		// 获取延时的是String数据，强转int
		waitTime = Integer.valueOf(px.getElementtext("/*/waitTime"));
		userad = px.getElementtext("/*/adminuser");
		pwd=px.getElementtext("/*/passpwd");
		asp=px.getElementtext("/*/asp");
		app=px.getElementtext("/*/app");
		Package=px.getElementtext("/*/appPackage");
		Activity=px.getElementtext("/*/appActivity");
		
	}
}
