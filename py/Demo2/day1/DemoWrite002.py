#coding=utf-8
print("写文件，写内容，关流");
p=open("b.txt","w");
p.write("b.txt写文件,w");
p.close();


print("读文件");
#读文件
r=open("b.txt","r");
r.seek(0)
re=r.read(100);
r.close();
print(re);







