#!/bin/bash

# Script to find components with potential memory leaks
# Searches for .subscribe() calls that don't have takeUntil

echo "==================================================================="
echo "Finding Components with Potential Memory Leaks"
echo "==================================================================="
echo ""
echo "Searching for .subscribe() without takeUntil()..."
echo ""

# Find all TypeScript files with .subscribe()
FILES=$(grep -r "\.subscribe(" src/app --include="*.ts" | grep -v "node_modules" | grep -v "\.spec\.ts" | cut -d: -f1 | sort -u)

TOTAL=0
NEEDS_FIX=0

for FILE in $FILES; do
    TOTAL=$((TOTAL + 1))

    # Check if file has takeUntil
    if ! grep -q "takeUntil" "$FILE"; then
        # Check if it extends DestroyableComponent
        if ! grep -q "extends DestroyableComponent" "$FILE"; then
            echo "❌ $FILE"
            NEEDS_FIX=$((NEEDS_FIX + 1))
        else
            echo "⚠️  $FILE (extends DestroyableComponent but missing takeUntil)"
            NEEDS_FIX=$((NEEDS_FIX + 1))
        fi
    else
        echo "✅ $FILE"
    fi
done

echo ""
echo "==================================================================="
echo "Summary:"
echo "==================================================================="
echo "Total files with subscriptions: $TOTAL"
echo "Files needing fixes: $NEEDS_FIX"
echo "Files already fixed: $((TOTAL - NEEDS_FIX))"
echo ""
echo "To fix a component:"
echo "1. Import: import { DestroyableComponent } from 'src/app/core/base/destroyable.component';"
echo "2. Import: import { takeUntil } from 'rxjs';"
echo "3. Extend: export class MyComponent extends DestroyableComponent"
echo "4. Add to pipe: .pipe(takeUntil(this.destroy\$))"
echo ""
echo "See MEMORY_LEAK_FIX_GUIDE.md for detailed instructions"
echo "==================================================================="
