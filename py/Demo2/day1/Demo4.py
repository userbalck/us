#condig=utf-8
a=[1,2,3,4,5,6,7];
print(
'''
正向索引
反向索引
默认索引
'''
);
print(a)
#正向索引
print(a[0:4:1]);

#反响索引
print(a[-1:-4:-1]);
##  默认索引
print(a[:]);

##添加列表
b=[1,2,3,];
c=[4,5,6,7,8];
print(id(b));
print(id(c));
print(b+c);
print(c+b);
#使用extend方法,把c添加到b列表内,地址值围边
print(b,c)
b.extend(c);
print(b);
print(id(b))
#添加任意对象到列表的末端
b.append(c);
print(b);

