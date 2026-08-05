#!/bin/sh

if [ ! -f package.json ]; then
  npx -p @angular/cli ng new app-temp --directory . --defaults --skip-git
fi

if [ ! -d "node_modules" ] || [ ! -f "node_modules/.bin/ng" ]; then
	npm install
fi

exec npm start -- --host 0.0.0.0 --poll 1000