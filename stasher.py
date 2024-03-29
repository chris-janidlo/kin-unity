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
    run(f'git branch -D {branch}_wip')
    run(f'git switch -c {branch}_wip')
    run('git stash apply')
    run('git add .')
    run('git commit --no-verify -m wip')
    run('git push -f')
    run(f'git switch {branch}')


def down():
    check_git_is_clean()
    branch = get_branch()
    run(f'git branch -D {branch}_wip')
    run(f'git switch -c {branch}_wip')
    run(f'git pull origin {branch}_wip')
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
    print("> " + cmd)
    args = shlex.split(cmd)
    subprocess.run(args).check_returncode()


def check_git_is_clean():
    result = subprocess.run(
        shlex.split('git status'),
        capture_output=True,
        encoding='utf-8'
    )
    result.check_returncode()
    out = result.stdout

    if "up to date" not in out:
        print("branch is not up to date")
        exit(1)

    if "nothing to commit" not in out or "working tree clean" not in out:
        print("you have unstaged or uncommitted changes")
        exit(1)


if __name__ == '__main__':
    main()
