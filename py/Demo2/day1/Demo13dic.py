#conding=utf-8
##习题7：

'''
#用字典的方式完成下面一个小型的学生管理系统。
1 学生有下面几个属性：姓名，年龄，考试分数包括：语文，数学，英语得分。
比如定义2个同学：

姓名：李明，年龄25，考试分数：语文80，数学75，英语85
姓名：张强，年龄23，考试分数：语文75，数学82，英语78
'''
seudentinfo={"liming":{"name":"李明","age":25,"fengshu":{"chinse":89,"englisth":85,"math":80}}};
print(seudentinfo);
seudentinfo["zhangqiang"]={"zhangqiang":"张强","age":23,"fengshu":{"chinse":23,"engein":45,"math":64}};
print(seudentinfo);

#2 给学生添加一门python课程成绩，李明60分，张强：80分
seudentinfo["liming"]["fengshu"]["python"]=60;
print(seudentinfo);
seudentinfo["zhangqiang"]["fengshu"]["python"]=80;
print(seudentinfo);

#把张强的数学成绩由82分改成89分
seudentinfo["zhangqiang"]["fengshu"]["math"]=89;
print(seudentinfo);

#删除李明的年龄数据
del seudentinfo["liming"]["age"];
print(seudentinfo)

#对张强同学的课程分数按照从低到高排序输出。
f=seudentinfo["zhangqiang"]["fengshu"].values();
#f.sort();
print(f);

#外部删除学生所在的城市属性，不存在返回字符串 beijing
fd=seudentinfo.pop("chts","beijing");
print(fd);







