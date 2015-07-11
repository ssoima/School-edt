TARGET=asciiclient.jar
RUNFILE=asciiclient

CLASSES=Arguments.class Assignment4.class
LFILES=$(addprefix -C src , $(CLASSES))
OBJECTS=$(addprefix src/, $(CLASSES))
SOURCES=$(OBJECTS:.class=.java)

export LANG = en_US.utf8

all: $(TARGET) $(RUNFILE)

.PHONY: all clean fresh

$(TARGET): $(OBJECTS)
	jar cfm $(TARGET) deps/manifest.txt $(LFILES)

%.class: %.java
	javac $(SOURCES)

$(RUNFILE):
	cp deps/run $(RUNFILE)
	chmod +x $(RUNFILE)

clean:
	-rm -f $(OBJECTS)
	-rm -f $(TARGET)
	-rm -f $(RUNFILE)

fresh: clean all
