"""This is a stupid hack around Cities: Skylines caching the UI classes
so that UI components can be updated without having to restart the game.

Visual Studio will run this script before compiling, and this script will look
for OptionsPanel<10 digits> and change the digits so that the game will load
the new class. The script will do nothing if the class is just "OptionsPanel",
and it should be checked into the repository as "OptionsPanel".
"""

import re
import time

file_paths = [
  r'ui\OptionsPanel.cs',
  r'ui\StreetDirectionViewerUI.cs',
]

n = int(time.time())  # seconds after epoch
new_class_name = 'OptionsPanel%s' % n

for file_path in file_paths:
  with open(file_path) as f:
    content = f.read()
  content = re.sub('OptionsPanel\d{10}', new_class_name, content)
  with open(file_path, 'w') as f:
    f.write(content)
