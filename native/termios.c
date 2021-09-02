#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <termios.h>
#include <stdarg.h>

__attribute__((noreturn)) static void die(char *fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	vfprintf(stderr, fmt, ap);
	va_end(ap);
	exit(1);
}

int main(int argc, char **argv)
{
	int res;
	int fd;
	int enable;
	struct termios t;

	if (argc < 2)
		die("missing argument\n");

	switch ((int)argv[1][0]) {
	case (int)'0':
		enable = 0;
		break;
	case (int)'1':
		enable = 1;
		break;
	default:
		die("invalid argument (must be 0 or 1)\n");
	}

	if ((fd = open("/dev/tty", O_RDWR)) < 0)
		die("failed to open /dev/tty file: %d\n", fd);

	printf("\nfd: %d\n", fd);

	if ((res = tcgetattr(fd, &t)))
		die("tcgetattr failed: %d\n", res);

	printf("before: 0x%lx\n", t.c_lflag);

	if (enable)
		t.c_lflag |= ECHO;
	else
		t.c_lflag &= ~ECHO;

	printf("after: 0x%lx\n", t.c_lflag);

	if ((res = tcsetattr(fd, TCSAFLUSH, &t)))
		die("tcsetattr failed: %d\n", res);

	return 0;
}
