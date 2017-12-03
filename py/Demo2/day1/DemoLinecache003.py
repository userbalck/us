import linecache;  #导入模块
#直接读取行数文本
p=open("b.txt","w");
p.write("hell \nweinio\nheishou");
p.close();
g=linecache.getline("b.txt",1); #读取行数
g=linecache.getline("b.txt",2);
print(g)  #输出行内容
lines=linecache.getlines("b.txt")
print(lines);#输出全部文件文件内容