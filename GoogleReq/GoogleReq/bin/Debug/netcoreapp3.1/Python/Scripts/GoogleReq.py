from googlesearch import *
import argparse
from os import path

parser = argparse.ArgumentParser()
parser.add_argument("-results", default="10")
args = parser.parse_args()

numbers = args.results

startpath = path.join(path.dirname(__file__), 'configpath.txt')

with open(startpath, 'r') as config:
    path = config.read()
    print(path)

with open(path + "Query.txt", 'r') as f:
    print("Getting search query")
    Query = f.read()
    f.close()
with open(path + "langsettings.txt", 'r') as f:
    print("Getting langauge settings")
    lang = f.read()
    f.close()
print(f"SETTINGS: {Query} : {lang}")

for Result in search(Query, lang=lang, num_results=int(numbers)):
    print(Result)
