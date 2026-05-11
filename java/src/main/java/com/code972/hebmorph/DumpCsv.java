package com.code972.hebmorph;

import com.code972.hebmorph.datastructures.DictHebMorph;
import com.code972.hebmorph.datastructures.DictRadix;
import com.code972.hebmorph.hspell.HSpellDictionaryLoader;

import java.io.File;
import java.io.FileWriter;
import java.io.PrintWriter;
import java.lang.reflect.Field;
import java.util.ArrayList;
import java.util.List;

public class DumpCsv {
    public static void main(String[] args) throws Exception {
        System.out.println("Loading HSpell dictionary...");
        DictionaryLoader loader = new HSpellDictionaryLoader();
        DictHebMorph dictHebMorph = loader.loadDictionaryFromPath("../hspell-data-files/");

        Field dictField = DictHebMorph.class.getDeclaredField("dict");
        dictField.setAccessible(true);
        DictRadix<MorphData> dict = (DictRadix<MorphData>) dictField.get(dictHebMorph);

        System.out.println("Dictionary loaded. Dumping to CSV...");
        File outputFile = new File("../HebMorphCsharp/HebMorph.WorkerService/gold_dictionary.csv");
        outputFile.getParentFile().mkdirs();
        try (PrintWriter writer = new PrintWriter(new FileWriter(outputFile))) {
            writer.println("Form,Lemma,DescFlagValue,PrefixTypeValue");
            traverse(dict.getRootNode(), "", writer);
        }
        System.out.println("Dump complete.");
    }

    private static void traverse(DictRadix<MorphData>.DictNode node, String prefix, PrintWriter writer) {
        if (node == null) return;

        String currentStr = prefix;
        if (node.getKey() != null) {
            currentStr += new String(node.getKey());
        }

        MorphData data = node.getValue();
        if (data != null && data.getLemmas() != null) {
            for (MorphData.Lemma lemma : data.getLemmas()) {
                writer.println(String.format("%s,%s,%d,%d",
                        currentStr,
                        lemma.getLemma(),
                        lemma.getDescFlag().getVal(),
                        lemma.getPrefix().getValue()));
            }
        }

        if (node.getChildren() != null) {
            for (DictRadix<MorphData>.DictNode child : node.getChildren()) {
                if (child != null) {
                    traverse(child, currentStr, writer);
                }
            }
        }
    }
}
