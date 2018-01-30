# -*- coding: utf-8 -*-
import urllib;
import os;
import random;


def sava_url_content(url,folder_path=None):
    if not (url.startswith("http://") or url.startswith("https://")):
        return u"url网址不合法"
    try:

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

def get_url_siez(url):
    #获取href统计出现多少次
    if not (url.startswith("http://") or url.startswith("https://")):
        return u"url网址不合法"
    d=urllib.urlopen(url);
    content= d.read();  #读取

    lenint=len(content.split("<a href=")) - 1
    return lenint;

print(get_url_siez("http://www.baidu.com/"));

#注意路径符号
'''
print(sava_url_content("http://www.baidu.com/","E:/py/Demo2/day2"));
'''

