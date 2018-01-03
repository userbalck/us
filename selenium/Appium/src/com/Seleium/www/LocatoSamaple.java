package com.Seleium.www;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.util.HashMap;
import java.util.Map;

import org.ho.yaml.Yaml;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedCondition;
import org.openqa.selenium.support.ui.WebDriverWait;

import io.appium.java_client.AppiumDriver;

public class LocatoSamaple {
	private static final String Testxpath= null;
	private String yamlfile;
	private AppiumDriver driver;
	private Map<String, Map<String, String>> ml;

	public LocatoSamaple(AppiumDriver driver) {
		yamlfile = "Testxpath"; //�ļ���ֵ
		this.getYamlFile();
		this.driver = driver;
	}

	// ��ȡ�ļ�1
	@SuppressWarnings("unchecked")
	private void getYamlFile() {
		File f = new File("locato/"+yamlfile+".yaml");
		try {
			ml = Yaml.loadType(new FileInputStream(f.getAbsolutePath()), HashMap.class);

		} catch (FileNotFoundException e) {
			// TODO: handle exception
			e.printStackTrace();
		}
		System.out.println("getYAML");
	}

	// �ж�
	private By getBy(String type, String value) {
		By by = null;
		if (type.equals("id")) {
			by = By.id(value);
		}
		if (type.equals("xpath")) {
			by = By.xpath(value);
		}
		return by;
	}

	// ��ӵ�map����
	public WebElement getElement(String key) {
		String type = ml.get(key).get("type");
		String value = ml.get(key).get("value");
		// return driver.findElement(this.getBy(type, value)); // ֱ��ȥ��Ԫ�أ�����������
		return this.waitForElement(this.getBy(type, value)); // ���õȴ�ʱ�䷽��
	}

	// �ȴ�Ԫ��ʱ��
	private WebElement waitForElement(final By by) {
		WebElement element = null;
		//int waitTime = config.waitTime;
		int waitTime=10;
		try {
			element = new WebDriverWait(driver, waitTime).until(new ExpectedCondition<WebElement>() {
				public WebElement apply(WebDriver d) {
					return d.findElement(by);
				}
			});
		} catch (Exception e) {
			System.out.println(by.toString() + "������" + waitTime);
		}
		return element;

	}

}
