const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const html = fs.readFileSync('index.html', 'utf8');
const initial = html.match(/const INITIAL_BOARD = \[[\s\S]*?\];/)[0];
const functions = html.slice(html.indexOf('    function parseCoordinate('), html.indexOf('    function renderMoveList('));
const context = vm.createContext({});
vm.runInContext(initial + '\nlet movesList = []; let moveHistory = [];\n' + functions, context);
function replay(moves) {
  context.input = moves.split(';').filter(Boolean);
  return vm.runInContext('movesList = input; buildMoveHistory(); moveHistory', context);
}
let history = replay('E2-E4;A7-A6;E4-E5;D7-D5;E5-D6');
assert.equal(history[5][3][3], '');
assert.equal(history[5][2][3], 'P');
assert.equal(history[4][3][3], 'p');
history = replay('A2-A3;E7-E5;A3-A4;E5-E4;D2-D4;E4-D3');
assert.equal(history.length, 7);
assert.equal(history[6][4][3], '');
assert.equal(history[6][5][3], 'p');
const prefix = 'A2-A4;H7-H5;A4-A5;H5-H4;A5-A6;H4-H3;A6-B7;H3-G2;';
for (const type of ['Q', 'R', 'B', 'N']) {
  history = replay(prefix + 'B7-A8=' + type + ';G2-H1=' + type);
  assert.equal(history[10][0][0], type);
  assert.equal(history[10][7][7], type.toLowerCase());
  assert.equal(history[8][1][1], 'P');
}
history = replay(prefix + 'B7-A8;G2-H1');
assert.equal(history[10][0][0], 'Q');
assert.equal(history[10][7][7], 'q');
console.log('Passed web replay checks (both colors, all promotions, legacy files and history).');
