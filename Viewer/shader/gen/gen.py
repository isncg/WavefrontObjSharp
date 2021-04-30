import os
from jinja2 import Environment, FileSystemLoader
files = os.listdir('..')
shaderFileNameDic = dict()
shaderUniformDic = dict()
for name in files:
    sp = name.split('.')
    if len(sp) == 2:
        if sp[0] not in shaderFileNameDic:
            shaderFileNameDic[sp[0]] = []
        shaderFileNameDic[sp[0]].append(name)
for k,v in shaderFileNameDic.items():
    print(k,v)
    shaderUniformDic[k] = []
    for f in v:
        lines = open('../'+f).readlines()
        for l in lines:
            sp = l.strip().split(' ')
            spr = []
            for s in sp:
                if len(s) > 0:
                    spr.append(s)
            if len(spr) >= 3 and spr[0] == 'uniform':
                shaderUniformDic[k].append((spr[1], spr[2]))

for k,v in shaderUniformDic.items():
    print(k,v)

loader = FileSystemLoader('./')
env = Environment(loader=loader)
template = env.get_template('template.txt')