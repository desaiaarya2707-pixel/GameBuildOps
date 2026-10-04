FROM alpine:3.20

WORKDIR /GameBuildOps

COPY . .

RUN echo "Checking Unity project structure..." && \
    test -d Assets && \
    test -d Packages && \
    test -d ProjectSettings && \
    echo "Unity project structure looks good!"

RUN echo "Checking Unity version file..." && \
    test -f ProjectSettings/ProjectVersion.txt && \
    grep -q "m_EditorVersion:" ProjectSettings/ProjectVersion.txt && \
    echo "Unity version file found!"

RUN echo "Checking GameBuildOps tests..." && \
    test -f Assets/Tests/GameBuildOpsTests.cs && \
    test -f Assets/Tests/Tests.asmdef && \
    grep -q "\[Test\]" Assets/Tests/GameBuildOpsTests.cs && \
    grep -q "Assert.AreEqual" Assets/Tests/GameBuildOpsTests.cs && \
    echo "GameBuildOps tests found!"

CMD ["sh", "-c", "echo '================================'; echo 'GameBuildOps Docker validation PASSED!'; echo '================================'"]