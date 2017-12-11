#coding=utf-8
#输出内容
f=open("test.txt","r",encoding="utf-8");
t=f.read();
f.close();
print(t);
#2输出内容长度
print("长度：",len(t));
#去除该文本换行
n=t.replace("\n","");
print(n);
# 4  替换其中2012为2013
th=n.replace("2012","2013");
print(th);
#5取出最最中间的长度为5的子串
#print (t[len(t)/2:len(t)/2+5].encode("utf-8"));
#请取出最后2个字符。
print(th[-2:]);
##7  请从字符串的最初开始，截断该字符串，使其长度为11.
print(th[:11]);

##8 请将4中的字符串保存为test1.py文本
print("th字符串="+th)
rinfo=th.replace("2013","2016");
f=open("test1.py","w");   ##打开文件没有就创建
f.write(rinfo); ##写文件
f.close();    ##关流
print(rinfo);

print("第五题--------------------")
#请用代码形式概述引用 机制，对象是从3开始，
import sys;

cinfo="123456";
print(id(cinfo));    #获取地址
print(sys.getrefcount("123456"));

binfo="123456";
print(id(binfo));
print(sys.getrefcount("123456"));

###6 已知如下代码

##字符串拼接
a="字符串拼接1";
b="字符串拼接2";
#方法1在做大量字符串拼接对象时候不推荐
c=a+b;
print(c);

##方法2





