#!/bin/sh -e

base="$(dirname "$0")"

# Check for required binaries in $PATH
for binary in dotnet jq; do
	if ! which "$binary" > /dev/null; then
		echo >&2 "error: no '$binary' executable in path"
		exit 1
	fi
done

# Run benchmark
dotnet run -c Release -f netcoreapp3.1 -p "$base"/Cottle.Benchmark -- --disableLogFile --exporters JSON --filter 'Cottle.Benchmark.Benchmarks.CompareBenchmark.*'

# Locate benchmark results in file
benchmark="$base/../benchmark.md"
line="$(grep -Fn 'var benchmarks =' "$benchmark" | cut -d : -f 1)"

if [ -z "$line" ]; then
	echo >&2 "error: cannot locate benchmark results in $benchmark"
	exit 1
fi

# Transform and output JSON result
result="$(mktemp)"

(
	head -n "$((line - 1))" "$benchmark"
	echo -n '        var benchmarks = '

	< BenchmarkDotNet.Artifacts/results/Cottle.Benchmark.Benchmarks.CompareBenchmark-report-full-compressed.json jq -cj '
.Benchmarks |
map([(.Parameters | sub("Engine="; "")), (.Method | ascii_downcase), (.Statistics.Mean | round)]) |
reduce .[] as $item ({}; . * {($item[0]): {($item[1]): $item[2]}})'

	echo ';'

	tail -n "+$((line + 1))" "$benchmark"
) > "$result"

mv "$result" "$benchmark"

echo >&2 "$benchmark was updated."
