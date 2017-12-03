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
#取出最最中间的长度为5的子串
#ut=th[len(th)/2:len(th)/2+5].encode("utf-8");

