package com.Seleium.www;

import java.io.File;
import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;
/*
 * ��ȡxml�ļ�
 */
public class Parsexml {
	private static Document document;
	String filePath;
	public void load(String filePath) {
		File file = new File(filePath); // �����ļ�·��
		if (file.exists()) { // �ж�·��
			SAXReader saxReader = new SAXReader();  
			try {
				document = saxReader.read(file);
			} catch (DocumentException e) {
				System.out.println("cath�ļ������쳣" + filePath);
			}
		} else {
			System.out.println("�ļ������쳣" + filePath);
		}

	}

	// ʹ�÷�������
	public static Element getElementObject(String elementpath) {
		return (Element) document.selectSingleNode(elementpath);

	}

	// ��ȡ�����ļ�����
	public Parsexml(String filePath) {
		this.filePath = filePath;
		this.load(this.filePath);
	}
	// ʹ�÷���ϸ�����Д�·����Ϊ��
		public String getElementtext(String elementpath) {
			Element element = this.getElementObject(elementpath);
			if (element != null)
				return element.getTextTrim();
			else
				return null;
		}
}
