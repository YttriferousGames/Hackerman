from sys import stdin
from time import monotonic_ns, sleep
import termios
from colorama import Fore, Style, init

codes = {
    "r": Fore.RED,
    "g": Fore.GREEN,
    "b": Fore.BLUE,
    "B": Style.BRIGHT,
    " ": Style.RESET_ALL,
}

def t():
    now = monotonic_ns()
    try:
        prev = t.prev
    except AttributeError:
        prev = now
    v = now - prev
    t.prev = now

    # 1e9 / 60
    # Elapsed time in frames
    return v // 16666667

# TODO allow backspace and EOF
if __name__ == "__main__":
    init()
    fd = stdin.fileno()
    oldattr = termios.tcgetattr(fd)
    newattr = termios.tcgetattr(fd)
    newattr[3] = newattr[3] & ~termios.ICANON & ~termios.ECHO
    termios.tcsetattr(fd, termios.TCSANOW, newattr)

    rec = []
    try:
        code = False
        while len(d := stdin.read(1)) == 1:
            # EOF
            if d == '\x04':
                break
            # DEL
            elif d == '\x7F':
                d = '\b'
            elif not code and d == '\t':
                code = True
                continue

            if code:
                code = False
                if d in codes:
                    print(codes[d], end="", flush=True)
                else:
                    continue
                d = '\t' + d
            # BS
            elif d == '\b':
                print("\b \b", end="", flush=True)
            else:
                print(d, end="", flush=True)

            if d:
                rec.append((t(), d))
    except KeyboardInterrupt:
        pass

    termios.tcsetattr(fd, termios.TCSAFLUSH, oldattr)
    speed = input(Style.RESET_ALL + "\nPlay back at speed multiplier: ")
    if speed:
        speed = 60.0 * float(speed)
    else:
        speed = 60.0

    try:
        for t, d in rec:
            if d == '\b':
                print("\b \b", end="", flush=True)
                code = True
            elif len(d) == 2 and d[1] in codes:
                print(codes[d[1]], end="", flush=True)
            else:
                print(d, end="", flush=True)
            sleep(t / speed)
    except KeyboardInterrupt:
        pass
    print(Style.RESET_ALL, end="", flush=True)

