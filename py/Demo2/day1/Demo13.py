#coding:utf-8
a = [11,22,24,29,30,32];

#把28插入到列表的末端
a.append(28);
print(a);

#在元素29后面插入元素57
a.insert(4,57);
print(a);

#3 把元素11修改成6
a[0]=6;
print(a);

#3 删除元素32
ap=a.pop(-2);
print(a,"-----",ap);

#对列表从小到大排序
a.sort();
print(a);

#习题2
b = [1,2,3,4,5];
#用2种方法输出下面的结果
# [1,2,3,4,5,6,7,8]
aa=b+[6,7,8];
print(aa);

b.extend([6,7,8])
print(b)

# 用列表的2种方法返回结果：[5,4]
c = [1,2,3,4,5];
print(c[-1:-3:-1])

d=[];
d.append(c.pop());
d.append(c.pop());
print(d)

#判断2是否在列表里d
bi=2 in c;
print(bi);   #返回true存在

##习题3：
b = [23,45,22,44,25,66,78]
#生成所有奇数组成的列表
for m in b:
    if m%2==1:
        print(m);

#输出结果: ['the content 23','the content 45']
print("--the content 23','the content 45----");

for m in b[0:2:1]:
    print("the conntent %s" % m);

#输出结果: [25, 47, 24, 46, 27, 68, 80]
sb = [23,45,22,44,25,66,78];
print([m+2 for m in sb]);

#用range方法和列表推导的方法生成列表[11,22,33]， 有问题。
ra=range(11,34,11);
print(ra);

##习题5：已知元组:
a = (1,4,5,6,7);
# 判断元素4是否在元组里
print(4 in a);

#2 把元素5修改成8
b=list(a);
b[2]=8;
print(b);
d=tuple(b);   #//转换成元组
print(d);


##习题6：已知集合:setinfo = set('acbdfem')和
# 集合finfo = set('sabcdef')完成下面操作：
setinfo = set('acbdfem');
finfo = set('sabcdef');

#1 添加字符串对象'abc'到集合setinfo
setinfo.add("abc");
print(setinfo);

#2 删除集合setinfo里面的成员m
setinfo.remove("m");
print(setinfo);

#求2个集合的交集和并集
print(setinfo&finfo);
print(setinfo|finfo);





