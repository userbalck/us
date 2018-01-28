#conding=utf-8
#排序
import operator
a=[1,4,5,6,2,4,56,2];
a.sort();
print(a);

#反排序
a.sort(key=int,reverse=True);
print(a);

#同时2个排序
a=[(1,2,4),(3,4,5),(0,1,2)];
print(a)
a.sort(key=operator.itemgetter(1,2));
print(a);

#字符串模板应用
"%s is a $s" %("ni","heo");
print();