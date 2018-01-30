#coding=utf-8
api="http://api.baiyug.cn/vip/index.php?url=";
print(
'''
---------作者：balck_hs
--------免费看各大视频vip电影
例：http://www.iqiyi.com/v_19rr7plfzs.html#vfrm=19-9-0-1 
'''
);

def get_url():

    while True:
        url = raw_input("输入vip视频地址:");
        print("while")
        if not (url.startswith("http://") or url.startswith("https://")):
            print(u"输入vip视频地址错误，请重新输入");
        else:
            break;
    url = api + url;
    return url;


ht=get_url();
print(
'''
1、复制以下网址输入浏览器可以免费看vip视频
2、在当前目录下生成了一个txt文件的解析地址
'''+ht
);
p = open("play.txt", "w");
p.write("123456");
print("write")
p.close();
raw_input("输入任意内容关闭脚本....")
