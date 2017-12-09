#condio=UTF-8
'''
一.已经字符串 s = "i,am,lilei",请用两种办法取出之间的“am”字符。

二.在python中，如何修改字符串？

三.bool("2012" == 2012) 的结果是什么。

四.已知一个文件 test.txt，内容如下：

'''
print("第一题----------用两种办法取出之间的“am”字符------------------")
s="i,am,lilei,hiue,jhlsdj";
#方法1：
s=s [2:4];
print(s);
##方法2
i="i,am,lilei,hiue,jhlsdj";
c=i.split(",")[1];
print(c);

print("第二题---------修改字符串-------------------")
f="i love php";
print("源字符串：",f);
r=f.replace("php","jsp");
print(r)

print("第三题------修改字符串--bool2012 == 2012 的结果是什么--------")
bo=bool ="2012"==2012;
print(bo);



