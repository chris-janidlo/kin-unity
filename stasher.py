#!/usr/bin/env python3

import argparse
from datetime import datetime
import subprocess
import shlex


def main():
    parser = argparse.ArgumentParser(
        description='hack for quickly synching WIPs between computers',
        formatter_class=argparse.ArgumentDefaultsHelpFormatter
    )
    parser.add_argument(
        'direction',
        choices=['up', 'down'],
        help='up = save working state to repo, down = get working state from repo'
    )
    args = parser.parse_args()

    match args.direction:
        case 'up':
            up()
        case 'down':
            down()


def up():
    branch = get_branch()
    run(f'git stash push -u -m "stasher auto-stash {datetime.now()}"')
    run(f'git switch -C {branch}_wip')
    run('git stash apply')
    run('git add .')
    run('git commit --no-verify -m wip')
    run('git push -f')
    run(f'git switch {branch}')


def down():
    branch = get_branch()
    run(f'git switch -C {branch}_wip')
    run('git pull')
    run('git reset HEAD^')
    run(f'git switch {branch}')


def get_branch():
    result = subprocess.run(
        shlex.split('git rev-parse --abbrev-ref HEAD'),
        capture_output=True,
        encoding='utf-8'
    )
    result.check_returncode()
    return result.stdout.strip()


def run(cmd):
    args = shlex.split(cmd)
    subprocess.run(args).check_returncode()


if __name__ == '__main__':
    main()
