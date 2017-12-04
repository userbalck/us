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
#替换其中2012为2013
th=n.replace("2012","2013");
print(th);
#5取出最最中间的长度为5的子串
#print (t[len(t)/2:len(t)/2+5].encode("utf-8"));
#请取出最后2个字符。
print(th[-2:]);
#请从字符串的最初开始，截断该字符串，使其长度为11.
print(th[:11]);


