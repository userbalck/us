# -*- coding: utf-8 -*-
import urllib;
import os;
import random;


def sava_url_content(url,folder_path=None):
    try:
        print(url)
        d=urllib.urlopen(url);
        print(d)
    except Exception as e:
        return  u"该网友无法打开";
    else:
        content= d.read();  #读取
    if not os.path.isdir(folder_path):
        return u"folder_path不是文件夹";
    rand_filename="TEST_%s"%random.randint(1,1000);   #生成一个随机数，可控的范围
    file_path=os.path.join(folder_path,rand_filename);
    d=open(file_path,"w");
    d.write(content);
    d.close();
    return file_path;


def sava_url(url,folder_path=None):
    if not (url.startswith('http://') or url.startswith('https://')):
        return u'url地址不符合规格'
    if not os.path.isdir(folder_path):
        return u'folder_path非文件夹'

    d = urllib.urlopen(url)
    return "svaurl";
    content = d.read()
    rand_filename = 'test_%s' % random.randint(1, 1000);


#print(sava_url("https://www.baidu.com/","E:/pys"));

print(sava_url_content("https://www.baidu.com/","E:/py"));
