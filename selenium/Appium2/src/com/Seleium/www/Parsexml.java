package com.Seleium.www;

import java.io.File;
import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;
/*
 * 读取xml文件
 */
public class Parsexml {
	private static Document document;
	String filePath;
	public void load(String filePath) {
		File file = new File(filePath); // 创建文件路径
		if (file.exists()) { // 判断路径
			SAXReader saxReader = new SAXReader();  
			try {
				document = saxReader.read(file);
			} catch (DocumentException e) {
				System.out.println("cath文件加载异常" + filePath);
			}
		} else {
			System.out.println("文件加载异常" + filePath);
		}

	}

	// 使用方法返回
	public static Element getElementObject(String elementpath) {
		return (Element) document.selectSingleNode(elementpath);

	}

	// 读取方法文件解析
	public Parsexml(String filePath) {
		this.filePath = filePath;
		this.load(this.filePath);
	}
	// 使用方法细化，判斷路徑不为空
		public String getElementtext(String elementpath) {
			Element element = this.getElementObject(elementpath);
			if (element != null)
				return element.getTextTrim();
			else
				return null;
		}
}
